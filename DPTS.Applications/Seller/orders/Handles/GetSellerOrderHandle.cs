using DPTS.Applications.Seller.orders.Dtos;
using DPTS.Applications.Seller.orders.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.orders.Handles
{
    public class GetSellerOrderHandle : IRequestHandler<GetSellerOrderQuery, ServiceResult<IEnumerable<OrderListItemDto>>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ILogger<GetSellerOrderHandle> _logger;

        public GetSellerOrderHandle(
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            ILogger<GetSellerOrderHandle> logger)
        {
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<OrderListItemDto>>> Handle(GetSellerOrderQuery query, CancellationToken token = default)
        {
            try
            {
                // Lấy tất cả đơn hàng có escrow và buyer liên quan đến seller
                var orders = (await _orderRepository.GetsAsync(includeBuyer: true, includeEscrow: true))
                    .Where(x => x.Escrow.SellerId == query.SellerId)
                    .ToList();

                // Lấy tất cả order item có product thuộc seller
                var orderItems = (await _orderItemRepository.GetsAsync(includeProduct: true))
                    .Where(x => x.Product.SellerId == query.SellerId)
                    .ToList();

                List<ResultJoined> resultJoineds;

                try
                {
                    // Join order và orderItem theo OrderId để gom thông tin hiển thị
                    resultJoineds = (
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
                        }
                    ).ToList();
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi join dữ liệu giữa orders và orderItems.");
                    return ServiceResult<IEnumerable<OrderListItemDto>>.Error("Không thể xử lý dữ liệu đơn hàng.");
                }

                // Dựng kết quả hiển thị từ dữ liệu đã join
                var result = resultJoineds
                    .OrderByDescending(x => x.BuyerAt)
                    .Select(x => new OrderListItemDto
                    {
                        OrderId = x.OrderId,
                        BuyerName = x.BuyerName,
                        ProductName = x.ProductName,
                        Amount = x.Amount,
                        DateBuyAt = x.BuyerAt.ToString("dd-MM-yyyy"),
                        TimeBuyAt = x.BuyerAt.ToString("HH:mm"),
                        Status = EnumHandle.HandleEscrowStatus(x.Status),
                    })
                    .OrderBy(x => x.OrderId)
                    .Skip((query.PageCount - 1) * query.PageSize)
                    .Take(query.PageSize);

                return ServiceResult<IEnumerable<OrderListItemDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unhandled exception in GetSellerOrderHandle.Handle.");
                return ServiceResult<IEnumerable<OrderListItemDto>>.Error("Đã xảy ra lỗi khi lấy danh sách đơn hàng.");
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
