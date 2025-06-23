using DPTS.Applications.Sellers.products.Dtos;
using DPTS.Applications.Sellers.products.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Sellers.products.Handles
{
    public class GetProductsHandler : IRequestHandler<GetSellerProductsItemQuery, ServiceResult<IEnumerable<ProductListItemDto>>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IEscrowRepository _escrowRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly ILogger<GetProductsHandler> _logger;
        private readonly IStoreRepository _storeRepository;

        public GetProductsHandler(
            IProductRepository productRepository,
            IEscrowRepository escrowRepository,
            IOrderItemRepository orderItemRepository,
            IOrderRepository orderRepository,
            IProductReviewRepository productReviewRepository,
            ILogger<GetProductsHandler> logger,
            IStoreRepository storeRepository)
        {
            _productRepository = productRepository;
            _escrowRepository = escrowRepository;
            _orderItemRepository = orderItemRepository;
            _orderRepository = orderRepository;
            _productReviewRepository = productReviewRepository;
            _logger = logger;
            _storeRepository = storeRepository;
        }

        public async Task<ServiceResult<IEnumerable<ProductListItemDto>>> Handle(GetSellerProductsItemQuery query, CancellationToken token = default)
        {
            _logger.LogInformation("Handling GetSellerProductsItemQuery for SellerId: {SellerId}", query.SellerId);

            try
            {
                var store = await _storeRepository.GetByUserIdAsync(query.SellerId);
                if (store == null)
                {
                    _logger.LogWarning("Store not found for SellerId: {SellerId}", query.SellerId);
                    return ServiceResult<IEnumerable<ProductListItemDto>>.Error("Không tìm thấy cửa hàng.");
                }

                var products = await _productRepository.GetsAsync(storeId: store.StoreId);
                var escrows = await _escrowRepository.GetsAsync(storeId: store.StoreId, status: Domains.EscrowStatus.Done);
                var orderItems = await _orderItemRepository.GetsAsync();

                // Tính tổng số lượng bán theo từng sản phẩm
                List<ProductIndexQuantity> soldQuantities;
                try
                {
                    soldQuantities = (
                        from escrow in escrows
                        where escrow.Order.IsPaid
                        join item in orderItems on escrow.Order.OrderId equals item.OrderId
                        group item by item.ProductId into g
                        select new ProductIndexQuantity
                        {
                            ProductId = g.Key,
                            TotalQuantity = g.Sum(x => x.Quantity)
                        }).ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to group sold quantity per product.");
                    return ServiceResult<IEnumerable<ProductListItemDto>>.Error("Không thể thống kê sản phẩm đã bán.");
                }

                var result = new List<ProductListItemDto>();

                foreach (var product in products)
                {
                    var productReviews = await _productReviewRepository.GetsAsync(productId: product.ProductId);
                    var reviewCount = productReviews.Count();
                    var totalRating = productReviews.Sum(x => x.Rating);

                    var quantitySold = soldQuantities.FirstOrDefault(x => x.ProductId == product.ProductId)?.TotalQuantity ?? 0;
                    var ratingAvg = reviewCount > 0 ? (double)totalRating / reviewCount : 0;

                    var primaryImage = product.Images.FirstOrDefault(x => x.IsPrimary)?.ImagePath ?? "no-image.jpg";

                    result.Add(new ProductListItemDto
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        Category = product.Category.CategoryName,
                        Price = product.Price,
                        Image = primaryImage,
                        QuantitySelled = quantitySold,
                        Status = EnumHandle.HandleProductStatus(product.Status),
                        Rating = ratingAvg
                    });
                }

                var paginatedResult = result
                    .OrderByDescending(x => x.QuantitySelled)
                    .Skip((query.PageCount - 1) * query.PageSize)
                    .Take(query.PageSize);

                _logger.LogInformation("Successfully returned {Count} products for SellerId: {SellerId}", result.Count, query.SellerId);
                return ServiceResult<IEnumerable<ProductListItemDto>>.Success(paginatedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in GetProductsHandler.");
                return ServiceResult<IEnumerable<ProductListItemDto>>.Error("Đã xảy ra lỗi khi lấy sản phẩm.");
            }
        }

        private class ProductIndexQuantity
        {
            public string ProductId { get; set; } = string.Empty;
            public int TotalQuantity { get; set; }
        }
    }
}
