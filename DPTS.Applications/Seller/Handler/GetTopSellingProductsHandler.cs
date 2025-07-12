//using DPTS.Applications.Sellers.revenues.Dtos;
//using DPTS.Applications.Sellers.revenues.Queries;
//using DPTS.Applications.Shareds;
//using DPTS.Infrastructures.Repository.Interfaces;
//using MediatR;
//using Microsoft.Extensions.Logging;

//namespace DPTS.Applications.Sellers.revenues.Handles
//{
//    public class GetTopSellingProductsHandler : IRequestHandler<GetTopSellingProductsQuery, ServiceResult<IEnumerable<TopSellingProductDto>>>
//    {
//        private readonly IOrderRepository _orderRepository;
//        private readonly IOrderItemRepository _itemRepository;
//        private readonly IProductRepository _productRepository;
//        private readonly ILogger<GetTopSellingProductsHandler> _logger;
//        private readonly IStoreRepository _storeRepository;

//        public GetTopSellingProductsHandler(
//            IOrderRepository orderRepository,
//            IOrderItemRepository itemRepository,
//            ILogger<GetTopSellingProductsHandler> logger,
//            IProductRepository productRepository,
//            IStoreRepository storeRepository)
//        {
//            _orderRepository = orderRepository;
//            _itemRepository = itemRepository;
//            _logger = logger;
//            _productRepository = productRepository;
//            _storeRepository = storeRepository;
//        }

//        public async Task<ServiceResult<IEnumerable<TopSellingProductDto>>> Handle(GetTopSellingProductsQuery query, CancellationToken cancellationToken = default)
//        {
//            _logger.LogInformation("Handling top selling products request for seller: {SellerId}", query.SellerId);

//            try
//            {
//                var store = await _storeRepository.GetByUserIdAsync(query.SellerId);
//                if (store == null)
//                {
//                    _logger.LogWarning("Store not found for seller: {SellerId}", query.SellerId);
//                    return ServiceResult<IEnumerable<TopSellingProductDto>>.Error("Không tìm thấy cửa hàng.");
//                }

//                // Lọc đơn đã hoàn tất
//                var orders = (await _orderRepository.GetsAsync(includeEscrow: true))
//                    .Where(x => x.Escrow.StoreId == store.StoreId && x.Escrow.Status == Domains.EscrowStatus.Done)
//                    .ToList();

//                // Lọc sản phẩm của seller
//                //var orderItems = (await _itemRepository.GetsAsync(includeProduct: true))
//                    .Where(x => x.Product.StoreId == store.StoreId)
//                    .ToList();

//                List<GroupResult> topSelling;

//                try
//                {
//                    var joined = from item in orderItems
//                                 join order in orders on item.OrderId equals order.OrderId
//                                 select new JoinedResult
//                                 {
//                                     ProductId = item.ProductId,
//                                     OrderId = order.OrderId,
//                                     OrderItemId = item.OrderItemId,
//                                     Quantity = item.Quantity,
//                                     TotalAmount = item.TotalAmount
//                                 };

//                    topSelling = joined
//                        .GroupBy(x => x.ProductId)
//                        .Select(g => new GroupResult
//                        {
//                            ProductId = g.Key ?? "",
//                            TotalQuantity = g.Sum(x => x.Quantity),
//                            TotalAmount = g.Sum(x => x.TotalAmount)
//                        })
//                        .OrderByDescending(x => x.TotalQuantity)
//                        .Take(3)
//                        .ToList();
//                }
//                catch (Exception ex)
//                {
//                    _logger.LogError(ex, "Error during grouping and joining order data.");
//                    return ServiceResult<IEnumerable<TopSellingProductDto>>.Error("Không thể xử lý dữ liệu sản phẩm bán chạy.");
//                }

//                var result = new List<TopSellingProductDto>();

//                foreach (var item in topSelling)
//                {
//                    var product = await _productRepository.GetByIdAsync(item.ProductId, includeImages: true);
//                    if (product == null)
//                    {
//                        _logger.LogWarning("Product not found: {ProductId}", item.ProductId);
//                        continue;
//                    }

//                    var imageUrl = product.Images
//                        .FirstOrDefault(x => x.IsPrimary)?.ImagePath ?? "no-image.jpg";

//                    result.Add(new TopSellingProductDto
//                    {
//                        ProductId = product.ProductId,
//                        ProductName = product.ProductName,
//                        OrderCount = item.TotalQuantity,
//                        Revenue = item.TotalAmount,
//                        ImageUrl = imageUrl
//                    });
//                }

//                _logger.LogInformation("Top selling products fetched successfully for seller: {SellerId}", query.SellerId);
//                return ServiceResult<IEnumerable<TopSellingProductDto>>.Success(result);
//            }
//            catch (Exception ex)
//            {
//                _logger.LogError(ex, "Unhandled exception in GetTopSellingProductsHandler.");
//                return ServiceResult<IEnumerable<TopSellingProductDto>>.Error("Lỗi khi lấy sản phẩm bán chạy.");
//            }
//        }

//        private class JoinedResult
//        {
//            public string? ProductId { get; set; }
//            public string? OrderId { get; set; }
//            public string? OrderItemId { get; set; }
//            public int Quantity { get; set; }
//            public decimal TotalAmount { get; set; }
//        }

//        private class GroupResult
//        {
//            public string ProductId { get; set; } = string.Empty;
//            public int TotalQuantity { get; set; }
//            public decimal TotalAmount { get; set; }
//        }
//    }
//}
