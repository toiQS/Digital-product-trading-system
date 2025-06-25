using DPTS.Applications.NoDistinctionOfRoles.products.Dtos;
using DPTS.Applications.NoDistinctionOfRoles.products.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.NoDistinctionOfRoles.products.Handles
{
    public class GetProductIndexListHandle : IRequestHandler<GetProductIndexListQuery, ServiceResult<IEnumerable<ProductIndexListDto>>>
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger<GetProductIndexListHandle> _logger;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        public GetProductIndexListHandle(IProductRepository productRepository, ILogger<GetProductIndexListHandle> logger, IOrderRepository orderRepository, IOrderItemRepository orderItemRepository)
        {
            _productRepository = productRepository;
            _logger = logger;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
        }

        public async Task<ServiceResult<IEnumerable<ProductIndexListDto>>> Handle(GetProductIndexListQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Bắt đầu xử lý GetProductIndexList với Text: {Text}, CategoryId: {CategoryId}, Rating: {Rating}", query.Text, query.CategoryId, query.Rating);
            try
            {
                var products = await _productRepository.GetsAsync(
                    text: query.Text,
                    categoryId: query.CategoryId,
                    includeCategory: true,
                    includeImages: true,
                    includeReviews: true);

                _logger.LogInformation("Đã lấy được {Count} sản phẩm từ repository.", products.Count());

                var orders = (await _orderRepository.GetsAsync(includeEscrow: true))
                    .Where(x => x.Escrow.Status == Domains.EscrowStatus.Done)
                    .ToList();

                var orderItems = await _orderItemRepository.GetsAsync();

                List<QueryResult> queryResults;
                try
                {
                    var queryResult = from order in orders
                                      join item in orderItems on order.OrderId equals item.OrderId
                                      group item by item.ProductId into grouped
                                      select new QueryResult
                                      {
                                          ProductId = grouped.Key,
                                          SumQuantity = grouped.Sum(x => x.Quantity)
                                      };
                    queryResults = queryResult.ToList();
                    _logger.LogInformation("Đã tính toán xong số lượng bán cho {Count} sản phẩm.", queryResults.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi tính tổng số lượng sản phẩm đã bán.");
                    return ServiceResult<IEnumerable<ProductIndexListDto>>.Error("Không thể tính số lượng sản phẩm đã bán.");
                }

                var productsResult = products.Select(p =>
                {
                    var rating = p.Reviews.Any() ? p.Reviews.Average(x => x.Rating) : 0;
                    var quantitySold = queryResults.FirstOrDefault(x => x.ProductId == p.ProductId)?.SumQuantity ?? 0;
                    return new ProductIndexListDto
                    {
                        ProductId = p.ProductId,
                        ProductName = p.ProductName,
                        Price = p.Price,
                        CategoryName = p.Category.CategoryName,
                        ProductImage = p.Images.FirstOrDefault(x => x.IsPrimary)?.ImagePath ?? "N/A",
                        RatingAverage = rating,
                        QuantitySold = quantitySold
                    };
                }).ToList();

                if (query.Rating > 0)
                {
                    productsResult = productsResult.Where(x => x.RatingAverage > query.Rating).ToList();
                    _logger.LogInformation("Lọc sản phẩm theo Rating > {Rating}, còn lại {Count} sản phẩm.", query.Rating, productsResult.Count);
                }

                var orderedResult = productsResult.OrderByDescending(p => p.QuantitySold).ToList();

                _logger.LogInformation("Hoàn thành xử lý GetProductIndexList. Tổng kết quả trả về: {Count}", orderedResult.Count);

                return ServiceResult<IEnumerable<ProductIndexListDto>>.Success(orderedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định trong GetProductIndexList.");
                return ServiceResult<IEnumerable<ProductIndexListDto>>.Error("Không thể lấy danh sách sản phẩm.");
            }
        }

        private class QueryResult
        {
            public string ProductId { get; set; } = string.Empty;
            public int SumQuantity { get; set; } = 0;
        }
    }
}
