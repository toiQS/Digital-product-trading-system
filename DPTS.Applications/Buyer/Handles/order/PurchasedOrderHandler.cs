using DPTS.Applications.Buyer.Dtos.order;
using DPTS.Applications.Buyer.Queries.order;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles.order
{
    public class PurchasedOrderHandler : IRequestHandler<GetPurchasedOrderQuery, ServiceResult<PurchasedOrderDto>>
    {
        private readonly IUserProfileRepository _profileRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<PurchasedOrderHandler> _logger;
        private readonly IEscrowRepository _ecrowRepository;

        public PurchasedOrderHandler(
            IUserProfileRepository profileRepository,
            IOrderRepository orderRepository,
            ILogger<PurchasedOrderHandler> logger,
            IEscrowRepository ecrowRepository)
        {
            _profileRepository = profileRepository;
            _orderRepository = orderRepository;
            _logger = logger;
            _ecrowRepository = ecrowRepository;
        }

        public async Task<ServiceResult<PurchasedOrderDto>> Handle(GetPurchasedOrderQuery request, CancellationToken cancellationToken)
        {
            var result = new PurchasedOrderDto();

            // 1. Kiểm tra người dùng
            var user = await _profileRepository.GetByUserIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("User not found. UserId={UserId}", request.UserId);
                return ServiceResult<PurchasedOrderDto>.Error("Không tìm thấy người dùng.");
            }

            // 2. Lấy danh sách đơn hàng
            var orders = await _orderRepository.GetByBuyerAsync(request.UserId);
            var totalOrder = orders.Count();
            int totalOrderDone = 0;
            int totalOrderPending = 0;
            int totalOrderError = 0;

            var allOrderItems = new List<OrderIndexDto>();

            foreach (var order in orders)
            {
                var escrows = await _ecrowRepository.GetByOrderIdAsync(order.OrderId);

                // 3. Gộp thông tin giao dịch (Escrow) thành các mục đơn hàng hiển thị
                foreach (var escrow in escrows)
                {
                    allOrderItems.Add(new OrderIndexDto
                    {
                        OrderId = order.OrderId,
                        Amount = escrow.Amount,
                        BuyAt = escrow.CreatedAt.ToString("dd/MM/yyyy"),
                        Status = EnumHandle.HandleEscrowStatus(escrow.Status)
                    });
                }

                // 4. Thống kê số lượng theo trạng thái
                totalOrderDone += escrows.Count(x => x.Status == Domains.EscrowStatus.Done);
                totalOrderPending += escrows.Count(x => x.Status == Domains.EscrowStatus.Pending);
                totalOrderError += escrows.Count(x => x.Status == Domains.EscrowStatus.Failed);
            }

            // 5. Gán thống kê tổng quan
            result.OrverViewIndexDtos.AddRange(new[]
            {
                new OrverViewIndexDto { Count = totalOrder, Name = "Tổng" },
                new OrverViewIndexDto { Count = totalOrderDone, Name = "Hoàn tất" },
                new OrverViewIndexDto { Count = totalOrderPending, Name = "Đang xử lý" },
                new OrverViewIndexDto { Count = totalOrderError, Name = "Lỗi" }
            });

            // 6. Gán danh sách đơn hàng có phân trang
            result.OrderIndexDtos = allOrderItems
                .OrderByDescending(x => x.BuyAt)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return ServiceResult<PurchasedOrderDto>.Success(result);
        }
    }
}
