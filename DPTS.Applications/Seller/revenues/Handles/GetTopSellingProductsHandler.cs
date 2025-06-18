using DPTS.Applications.Seller.revenues.Dtos;
using DPTS.Applications.Seller.revenues.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.revenues.Handles
{
    public class GetTopSellingProductsHandler : IRequestHandler<GetTopSellingProductsQuery, ServiceResult<IEnumerable<TopSellingProductDto>>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _itemRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<GetTopSellingProductsHandler> _logger;

        public GetTopSellingProductsHandler(
            IOrderRepository orderRepository,
            IOrderItemRepository itemRepository,
            ILogger<GetTopSellingProductsHandler> logger,
            IProductRepository productRepository)
        {
            _orderRepository = orderRepository;
            _itemRepository = itemRepository;
            _logger = logger;
            _productRepository = productRepository;
        }

        public async Task<ServiceResult<IEnumerable<TopSellingProductDto>>> Handle(GetTopSellingProductsQuery query, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Bắt đầu xử lý lấy danh sách top sản phẩm bán chạy nhất cho SellerId: {SellerId}", query.SellerId);

            try
            {
                // Lọc ra các đơn hàng đã hoàn tất của seller
                var orders = (await _orderRepository.GetsAsync(includeEscrow: true))
                    .Where(x => x.Escrow.SellerId == query.SellerId && x.Escrow.Status == Domains.EscrowStatus.Done)
                    .ToList();

                // Lấy các item có product thuộc seller
                var orderItems = (await _itemRepository.GetsAsync(includeProduct: true))
                    .Where(x => x.Product.SellerId == query.SellerId)
                    .ToList();

                List<GroupResult> topSelling;
                try
                {
                    // Join giữa order và orderItems
                    var joined = from item in orderItems
                                 join order in orders on item.OrderId equals order.OrderId
                                 select new JoinedResult
                                 {
                                     ProductId = item.ProductId,
                                     OrderId = order.OrderId,
                                     OrderItemId = item.OrderItemId,
                                     Quantity = item.Quantity,
                                     TotalAmount = item.TotalAmount
                                 };

                    // Gom nhóm theo ProductId và tính tổng quantity + doanh thu
                    var grouped = from j in joined
                                  group j by j.ProductId into g
                                  select new GroupResult
                                  {
                                      ProductId = g.Key ?? "",
                                      TotalQuantity = g.Sum(x => x.Quantity),
                                      TotalAmount = g.Sum(x => x.TotalAmount)
                                  };

                    topSelling = grouped
                        .OrderByDescending(x => x.TotalQuantity)
                        .Take(3)
                        .ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi join và gom nhóm sản phẩm đã bán.");
                    return ServiceResult<IEnumerable<TopSellingProductDto>>.Error("Không thể tính toán sản phẩm bán chạy.");
                }

                var result = new List<TopSellingProductDto>();

                foreach (var item in topSelling)
                {
                    var product = await _productRepository.GetByIdAsync(item.ProductId, includeImages: true);
                    if (product == null)
                    {
                        _logger.LogWarning("Không tìm thấy sản phẩm với ProductId = {ProductId}", item.ProductId);
                        continue;
                    }

                    var primaryImage = product.Images.FirstOrDefault(x => x.IsPrimary)?.ImagePath ?? "no-image.jpg";

                    result.Add(new TopSellingProductDto
                    {
                        ProductId = product.ProductId,
                        ProductName = product.ProductName,
                        OrderCount = item.TotalQuantity,
                        Revenue = item.TotalAmount,
                        ImageUrl = primaryImage
                    });
                }

                return ServiceResult<IEnumerable<TopSellingProductDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception trong GetTopSellingProductsHandler.");
                return ServiceResult<IEnumerable<TopSellingProductDto>>.Error("Lỗi khi lấy top sản phẩm bán chạy.");
            }
        }

        /// <summary>
        /// Kết quả sau khi join order và orderItem
        /// </summary>
        private class JoinedResult
        {
            public string? ProductId { get; set; }
            public string? OrderId { get; set; }
            public string? OrderItemId { get; set; }
            public int Quantity { get; set; }
            public decimal TotalAmount { get; set; }
        }

        /// <summary>
        /// Kết quả sau khi gom nhóm theo ProductId
        /// </summary>
        private class GroupResult
        {
            public string ProductId { get; set; } = string.Empty;
            public int TotalQuantity { get; set; }
            public decimal TotalAmount { get; set; }
        }
    }
}
