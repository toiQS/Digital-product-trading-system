using DPTS.Applications.Seller.orders.Dtos;
using DPTS.Applications.Seller.orders.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.orders.Handles
{
    public class GetSellerOrderHandle : IRequestHandler<GetSellerOrderQuery, ServiceResult<IEnumerable<OrderListItemDto>>>
    {
        private readonly IUserProfileRepository _profileRepository;
        private readonly IEscrowRepository _escrowRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<GetSellerOrderHandle> _logger;

        public GetSellerOrderHandle(
            IUserProfileRepository profileRepository,
            IEscrowRepository escrowRepository,
            IStoreRepository storeRepository,
            IOrderRepository orderRepository,
            ILogger<GetSellerOrderHandle> logger)
        {
            _profileRepository = profileRepository;
            _escrowRepository = escrowRepository;
            _storeRepository = storeRepository;
            _orderRepository = orderRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<OrderListItemDto>>> Handle(GetSellerOrderQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting seller orders for SellerId = {SellerId}", request.SellerId);

            // 1. Kiểm tra hồ sơ người bán
            var profile = await _profileRepository.GetByUserIdAsync(request.SellerId);
            if (profile == null)
            {
                _logger.LogWarning("User profile not found for SellerId = {SellerId}", request.SellerId);
                return ServiceResult<IEnumerable<OrderListItemDto>>.Error("Không tìm thấy thông tin người dùng.");
            }

            // 2. Lấy cửa hàng theo user
            var store = await _storeRepository.GetByUserIdAsync(profile.UserId);
            if (store == null)
            {
                _logger.LogWarning("Store not found for SellerId = {SellerId}", request.SellerId);
                return ServiceResult<IEnumerable<OrderListItemDto>>.Error("Không tìm thấy cửa hàng.");
            }

            // 3. Lấy các escrow thuộc cửa hàng
            var escrows = await _escrowRepository.GetAllAsync(storeId: store.StoreId);
            var result = new List<OrderListItemDto>();

            foreach (var escrow in escrows)
            {
                // 4. Tìm order gốc
                var order = await _orderRepository.GetByIdAsync(escrow.OrderId);
                if (order == null)
                {
                    _logger.LogWarning("Order not found. OrderId = {OrderId}", escrow.OrderId);
                    continue; // bỏ qua nếu lỗi order
                }

                // 5. Tìm thông tin người mua
                var buyer = await _profileRepository.GetByUserIdAsync(order.BuyerId);
                if (buyer == null)
                {
                    _logger.LogWarning("Buyer not found. BuyerId = {BuyerId}", order.BuyerId);
                    continue; // bỏ qua nếu lỗi buyer
                }

                // 6. Build kết quả từng đơn
                var item = new OrderListItemDto
                {
                    EscrowId = escrow.OrderId,
                    BuyerName = buyer.FullName ?? "Không rõ",
                    Amount = escrow.Amount,
                    DateBuyAt = escrow.CreatedAt.ToString("dd-MM-yyyy"),
                    TimeBuyAt = escrow.CreatedAt.ToString("HH:mm"),
                    Status = EnumHandle.HandleEscrowStatus(escrow.Status)
                };

                result.Add(item);
            }
            result.OrderByDescending(x =>  x.Amount).Skip((request.PageCount-1)*request.PageSize).Take(request.PageSize);

            //_logger.LogInformation("Successfully fetched {Count} orders for SellerId = {SellerId}", result.Count, request.SellerId);
            return ServiceResult<IEnumerable<OrderListItemDto>>.Success(result);
        }
    }
}
