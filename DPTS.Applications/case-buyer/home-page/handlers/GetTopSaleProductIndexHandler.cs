using DPTS.Applications.case_buyer.homepage.dtos;
using DPTS.Applications.case_buyer.homepage.models;
using DPTS.Applications.shareds;
using DPTS.Applications.shareds.maths;
using DPTS.Domains;
using DPTS.Infrastructures.Repositories.Contracts.AdjustmentRules;
using DPTS.Infrastructures.Repositories.Contracts.Escrows;
using DPTS.Infrastructures.Repositories.Contracts.OrderItems;
using DPTS.Infrastructures.Repositories.Contracts.Orders;
using DPTS.Infrastructures.Repositories.Contracts.ProductAdjustments;
using DPTS.Infrastructures.Repositories.Contracts.ProductImages;
using DPTS.Infrastructures.Repositories.Contracts.ProductReviews;
using DPTS.Infrastructures.Repositories.Contracts.Products;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.case_buyer.homepage.handlers
{
    public class GetTopSaleProductIndexHandler : IRequestHandler<GetTopSaleProductIndexQuery, ServiceResult<IEnumerable<TopSateProductIndexDto>>>
    {
        private readonly IProductReviewQuery _productReviewQuery;
        private readonly IProductImageQuery _productImageQuery;
        private readonly IProductQuery _productQuery;
        private readonly IOrderQuery _orderQuery;
        private readonly IOrderItemQuery _orderItemQuery;
        private readonly IEscrowQuery _escrowQuery;
        private readonly IProductAdjustmentQuery _productAdjustmentQuery;
        private readonly IAdjustmentRuleQuery _adjustmentRuleQuery;
        private readonly ILogger<GetTopSaleProductIndexHandler> _logger;

        public GetTopSaleProductIndexHandler(IProductReviewQuery productReviewQuery,
                                             IProductImageQuery productImageQuery,
                                             IProductQuery productQuery,
                                             IOrderQuery orderQuery,
                                             IOrderItemQuery orderItemQuery,
                                             IEscrowQuery escrowQuery,
                                             IProductAdjustmentQuery productAdjustmentQuery,
                                             IAdjustmentRuleQuery adjustmentRuleQuery,
                                             ILogger<GetTopSaleProductIndexHandler> logger)
        {
            _productReviewQuery = productReviewQuery;
            _productImageQuery = productImageQuery;
            _productQuery = productQuery;
            _orderQuery = orderQuery;
            _orderItemQuery = orderItemQuery;
            _escrowQuery = escrowQuery;
            _productAdjustmentQuery = productAdjustmentQuery;
            _adjustmentRuleQuery = adjustmentRuleQuery;
            _logger = logger;
        }

        async Task<ServiceResult<IEnumerable<TopSateProductIndexDto>>> IRequestHandler<GetTopSaleProductIndexQuery, ServiceResult<IEnumerable<TopSateProductIndexDto>>>.Handle(GetTopSaleProductIndexQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetTopSaleProductIndexQuery");
            IEnumerable<Escrow> doneEscrows = await _escrowQuery.GetDoneEscrowsAsync(cancellationToken);
            if (doneEscrows == null || !doneEscrows.Any())
            {
                _logger.LogWarning("No done escrows found.");
                return ServiceResult<IEnumerable<TopSateProductIndexDto>>.Error("Không tìm thấy giao dịch nào.");
            }
            List<string> orderIds = doneEscrows.Select(e => e.OrderId).Distinct().ToList();
            IEnumerable<OrderItem> orderItems = await _orderItemQuery.GetOrderItemsByOrderIdsAsync(orderIds, cancellationToken);

            if (orderItems == null || !orderItems.Any())
            {
                _logger.LogWarning("No order items found for the given order IDs.");
                return ServiceResult<IEnumerable<TopSateProductIndexDto>>.Error("Không tìm thấy sản phẩm nào.");
            }
            var groupedByProductId = orderItems.GroupBy(oi => oi.ProductId)
                .Select(g => new
                {
                    ProductId = g.Key,
                    TotalQuantity = g.Sum(oi => oi.Quantity)
                })
                .OrderByDescending(g => g.TotalQuantity)
                .Take(10);
            List<TopSateProductIndexDto> result = new();
            foreach (var item in groupedByProductId)
            {
                _logger.LogInformation($"ProductId: {item.ProductId}, TotalQuantity: {item.TotalQuantity}");
                Product? product = await _productQuery.GetByIdAsync(item.ProductId, cancellationToken);
                if (product == null)
                {
                    _logger.LogWarning($"Product with ID {item.ProductId} not found.");
                    continue;
                }
                ProductImage? imagePrimary = await _productImageQuery.GetPrimaryImageByProductIdAsync(item.ProductId, cancellationToken);
                if (imagePrimary == null)
                {
                    _logger.LogWarning($"Primary image for product ID {item.ProductId} not found.");
                    continue;
                }
                double averageRating = await _productReviewQuery.GetAverageRatingByProductIdAsync(item.ProductId, cancellationToken);
                if (averageRating < 0)
                {
                    _logger.LogWarning($"Average rating for product ID {item.ProductId} is invalid.");
                    continue;
                }
                int countRating = await _productReviewQuery.GetCountRatingByProductIdAsync(item.ProductId, cancellationToken);
                if (countRating < 0)
                {
                    _logger.LogWarning($"Count rating for product ID {item.ProductId} is invalid.");
                    continue;
                }
                IEnumerable<ProductAdjustment> productAdjustments = await _productAdjustmentQuery.GetsByProductIdAsync(item.ProductId, cancellationToken);
                if (productAdjustments.Count() > 1)
                {
                    _logger.LogWarning($"Multiple product adjustments found for product ID {item.ProductId}. Using the first one.");
                    return ServiceResult<IEnumerable<TopSateProductIndexDto>>.Error("Lỗi hệ thống, vui lòng thử lại sau.");

                }
                if (productAdjustments.Count() == 1)
                {
                    var productAdjustment = productAdjustments.FirstOrDefault();
                    if (productAdjustment == null)
                    {
                        _logger.LogWarning($"No product adjustment found for product ID {item.ProductId}. Using original price.");
                        return ServiceResult<IEnumerable<TopSateProductIndexDto>>.Error("Lỗi hệ thống, vui lòng thử lại sau.");
                    }
                    var adjustmentRule = await _adjustmentRuleQuery.GetByIdAsync(productAdjustment.RuleId, cancellationToken);
                    if (adjustmentRule == null)
                    {
                        _logger.LogWarning($"Adjustment rule with ID {productAdjustment.RuleId} not found. Using original price.");
                        return ServiceResult<IEnumerable<TopSateProductIndexDto>>.Error("Lỗi hệ thống, vui lòng thử lại sau.");
                    }
                    MathResult mathProduct = MathForProduct.CalculatePrice(product, adjustmentRule);
                    TopSateProductIndexDto dto = new()
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        ProductImage = imagePrimary.ImagePath,
                        Price = mathProduct.FinalPrice,
                        CountRating = countRating,
                        AverageRating = averageRating
                    };
                    result.Add(dto);
                }
                else
                {
                    TopSateProductIndexDto dto = new()
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        ProductImage = imagePrimary.ImagePath,
                        Price = product.OriginalPrice,
                        CountRating = countRating,
                        AverageRating = averageRating
                    };
                    result.Add(dto);
                }
            }
            return ServiceResult<IEnumerable<TopSateProductIndexDto>>.Success(result);
        }
    }
}
