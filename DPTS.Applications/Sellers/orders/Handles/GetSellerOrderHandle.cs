using DPTS.Applications.Sellers.orders.Dtos;
using DPTS.Applications.Sellers.orders.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Sellers.orders.Handles
{
    public class GetSellerOrderHandle : IRequestHandler<GetSellerOrderQuery, ServiceResult<IEnumerable<OrderListItemDto>>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ILogger<GetSellerOrderHandle> _logger;
        private readonly IStoreRepository _storeRepository;

        public GetSellerOrderHandle(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            ILogger<GetSellerOrderHandle> logger,
            IStoreRepository storeRepository)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _logger = logger;
            _storeRepository = storeRepository;
        }

        public async Task<ServiceResult<IEnumerable<OrderListItemDto>>> Handle(GetSellerOrderQuery query, CancellationToken token = default)
        {
            _logger.LogInformation("Bắt đầu xử lý GetSellerOrderQuery cho SellerId: {SellerId}", query.SellerId);

            try
            {
                var store = await _storeRepository.GetByUserIdAsync(query.SellerId);
                if (store == null)
                {
                    _logger.LogWarning("Không tìm thấy cửa hàng cho SellerId: {SellerId}", query.SellerId);
                    return ServiceResult<IEnumerable<OrderListItemDto>>.Error("Không tìm thấy cửa hàng.");
                }

                // Get orders with Escrow & Buyer included
                var orders = (await _orderRepository.GetsAsync(includeBuyer: true, includeEscrow: true))
                    .Where(x => x.Escrow.StoreId == store.StoreId)
                    .ToList();

                if (!string.IsNullOrWhiteSpace(query.Text))
                {
                    string pattern = $"%{query.Text.Trim()}%";
                    orders = orders.Where(x => EF.Functions.Like(x.Buyer.FullName ?? "", pattern)).ToList();
                }

                if (query.Status != 0) // If a specific status is passed
                {
                    orders = orders.Where(x => x.Escrow.Status == query.Status).ToList();
                }

                var orderItems = (await _orderItemRepository.GetsAsync(includeProduct: true))
                    .Where(x => x.Product.StoreId == store.StoreId)
                    .ToList();

                if (!string.IsNullOrWhiteSpace(query.Text))
                {
                    string pattern = $"%{query.Text.Trim()}%";
                    orderItems = orderItems.Where(x => EF.Functions.Like(x.Product.ProductName, pattern)).ToList();
                }

                var resultJoineds = (
                    from order in orders
                    join item in orderItems on order.OrderId equals item.OrderId
                    select new ResultJoined
                    {
                        OrderId = order.OrderId,
                        OrderItemId = item.OrderItemId,
                        ProductId = item.ProductId,
                        ProductName = item.Product.ProductName,
                        Price = item.Product.Price,
                        Quantity = item.Quantity,
                        BuyerId = order.BuyerId,
                        BuyerName = order.Buyer.FullName ?? "Không rõ",
                        BuyerAt = order.Escrow.CreatedAt,
                        EscrowId = order.Escrow.EscrowId,
                        Amount = order.Escrow.Amount,
                        Status = order.Escrow.Status
                    }).ToList();

                var result = resultJoineds
                    .OrderByDescending(x => x.BuyerAt)
                    .Skip((query.PageCount - 1) * query.PageSize)
                    .Take(query.PageSize)
                    .Select(x => new OrderListItemDto
                    {
                        OrderId = x.OrderId,
                        BuyerName = x.BuyerName,
                        ProductName = x.ProductName,
                        Amount = x.Amount,
                        DateBuyAt = x.BuyerAt.ToString("dd-MM-yyyy"),
                        TimeBuyAt = x.BuyerAt.ToString("HH:mm"),
                        Status = EnumHandle.HandleEscrowStatus(x.Status)
                    });

                return ServiceResult<IEnumerable<OrderListItemDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xảy ra trong GetSellerOrderHandle.");
                return ServiceResult<IEnumerable<OrderListItemDto>>.Error("Không thể lấy danh sách đơn hàng.");
            }
        }


        /// <summary>
        /// Class nội bộ để gom thông tin đơn hàng sau khi join order và orderItem
        /// </summary>
        private class ResultJoined
        {
            public string ProductId { get; set; } = string.Empty;
            public string ProductName { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public string BuyerId { get; set; } = string.Empty;
            public string BuyerName { get; set; } = string.Empty;
            public string OrderItemId { get; set; } = string.Empty;
            public string OrderId { get; set; } = string.Empty;
            public int Quantity { get; set; }
            public string EscrowId { get; set; } = string.Empty;
            public DateTime BuyerAt { get; set; }
            public decimal Amount { get; set; }
            public EscrowStatus Status { get; set; }
        }
    }
}
