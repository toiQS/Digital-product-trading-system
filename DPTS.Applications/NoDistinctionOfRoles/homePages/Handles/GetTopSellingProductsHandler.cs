using DPTS.Applications.NoDistinctionOfRoles.homePages.Dtos;
using DPTS.Applications.NoDistinctionOfRoles.homePages.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.NoDistinctionOfRoles.homePages.Handles
{
    public class GetTopSellingProductsHandler : IRequestHandler<GetTopSellingProductsQuery, ServiceResult<IEnumerable<TopSellingProductDto>>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly ILogger<GetTopSellingProductsHandler> _logger;

        public GetTopSellingProductsHandler(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            ILogger<GetTopSellingProductsHandler> logger,
            IProductReviewRepository productReviewRepository,
            IProductRepository productRepository,
            IProductImageRepository productImageRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _logger = logger;
            _productReviewRepository = productReviewRepository;
            _productRepository = productRepository;
            _productImageRepository = productImageRepository;
        }

        public async Task<ServiceResult<IEnumerable<TopSellingProductDto>>> Handle(GetTopSellingProductsQuery query,CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Bắt đầu truy vấn top sản phẩm bán chạy.");

            try
            {
                var products = await _productRepository.GetsAsync();
                var orders = await _orderRepository.GetsAsync(includeEscrow: true);
                var orderItems = await _orderItemRepository.GetsAsync();
                var productReviews = await _productReviewRepository.GetsAsync();
                var productImages = await _productImageRepository.GetsAsync(isPrimary: true);

                var validOrders = orders
                    .Where(x => x.Escrow != null && x.Escrow.Status == EscrowStatus.Done)
                    .Select(x => x.OrderId)
                    .ToHashSet();

                // Gộp dữ liệu
                var queryResults = (
                    from item in orderItems
                    where validOrders.Contains(item.OrderId)
                    join product in products on item.ProductId equals product.ProductId
                    let reviews = productReviews.Where(r => r.ProductId == product.ProductId).ToList()
                    let primaryImage = productImages.FirstOrDefault(img => img.ProductId == product.ProductId)?.ImagePath ?? ""
                    group new { item, product, reviews, primaryImage } by product.ProductId into g
                    select new QueryResult
                    {
                        ProductId = g.Key,
                        ProductName = g.First().product.ProductName,
                        Price = g.First().product.Price,
                        TotalSoldQuantity = g.Sum(x => x.item.Quantity),
                        ReviewCount = g.SelectMany(x => x.reviews).Count(),
                        AverageRating = g.SelectMany(x => x.reviews).Any()
                            ? g.SelectMany(x => x.reviews).Average(r => r.Rating)
                            : 0,
                        PrimaryImagePath = g.First().primaryImage
                    }
                ).ToList();

                _logger.LogInformation("Truy vấn thành công. Tổng số sản phẩm: {Count}", queryResults.Count);

                var result = queryResults
                    .OrderByDescending(x => x.TotalSoldQuantity)
                    .Take(10)
                    .Select(x => new TopSellingProductDto
                    {
                        ProductId = x.ProductId,
                        ProductName = x.ProductName,
                        Price = x.Price,
                        SoldQuantity = x.TotalSoldQuantity,
                        Rating = x.AverageRating,
                        ImageUrl = x.PrimaryImagePath
                    })
                    .ToList();

                return ServiceResult<IEnumerable<TopSellingProductDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi truy vấn top sản phẩm bán chạy.");
                return ServiceResult<IEnumerable<TopSellingProductDto>>.Error("Đã xảy ra lỗi khi lấy danh sách sản phẩm bán chạy.");
            }
        }

        private class QueryResult
        {
            public string ProductId { get; set; } = string.Empty;
            public string ProductName { get; set; } = string.Empty;
            public int TotalSoldQuantity { get; set; }
            public decimal Price { get; set; }
            public int ReviewCount { get; set; }
            public double AverageRating { get; set; }
            public string PrimaryImagePath { get; set; } = string.Empty;
        }
    }
}
