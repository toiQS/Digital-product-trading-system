using Azure.Core;
using DPTS.Applications.Admin.manage_order.dtos;
using DPTS.Applications.Admin.manage_order.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using LinqKit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Admin.manage_order.handlers
{
    public class GetOrdersHandler : IRequestHandler<GetOrdersQuery, ServiceResult<OrderDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetOrdersHandler> _logger; 
        private readonly IEscrowRepository _ecrowRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly ILogRepository _logRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IMessageRepository _messageRepository;

        public GetOrdersHandler(IUserRepository userRepository, ILogger<GetOrdersHandler> logger, IEscrowRepository ecrowRepository, IOrderRepository orderRepository, IUserProfileRepository userProfileRepository, IWalletTransactionRepository walletTransactionRepository, ILogRepository logRepository, IStoreRepository storeRepository, IWalletRepository walletRepository, IMessageRepository messageRepository
            )
        {
            _userRepository = userRepository;
            _logger = logger;
            _ecrowRepository = ecrowRepository;
            _orderRepository = orderRepository;
            _userProfileRepository = userProfileRepository;
            _walletTransactionRepository = walletTransactionRepository;
            _logRepository = logRepository;
            _storeRepository = storeRepository;
            _walletRepository = walletRepository;
            _messageRepository = messageRepository;
        }

        public async Task<ServiceResult<OrderDto>> Handle(GetOrdersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling get orders with condition");
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("Not found user with Id: {id}", request.UserId);
                return ServiceResult<OrderDto>.Error("Không tìm thấy người dùng");
            }
            if (user.RoleId != "Admin")
            {
                _logger.LogInformation("User current don't have enough access books");
                return ServiceResult<OrderDto>.Error("Người dùng hiện tại không đủ quyền truy cập");
            }
            var result = new OrderDto();
            var escrows = await _ecrowRepository.GetAllAsync();
            var syncEscrows = await SyncEscrow(escrows);
            if (syncEscrows.Status == StatusResult.Errored)
            {
                _logger.LogError(syncEscrows.MessageResult);
                return ServiceResult<OrderDto>.Error("Cập nhật đơn hàng không thành công");
            }
                
            if (request.Condition.RangeTime == RangeTime.Today)
            {
                
                escrows = escrows.Where(x => x.CreatedAt >= DateTime.Today && x.CreatedAt < DateTime.Today.AddDays(1));
            }
            if (request.Condition.RangeTime == RangeTime.Week)
            {
                var (startThisWeek, endThisWeek) = SharedHandle.GetWeekRange(0);
                escrows = escrows.Where(x => x.CreatedAt >= startThisWeek && x.CreatedAt <= endThisWeek);
            }
            if (request.Condition.RangeTime== RangeTime.Month)
            {
                var (startThisMonth, endThisMonth) = SharedHandle.GetMonthRange(0);
                escrows = escrows.Where(x => x.CreatedAt >= startThisMonth && x.CreatedAt < endThisMonth);
            }
            escrows.ForEach(async e =>
            {
                var order = await _orderRepository.GetByIdAsync(e.OrderId);
                if (order == null)
                {
                    _logger.LogError("Not found order with Id:{id}", e.OrderId);
                }
                var buyerProfile = await _userProfileRepository.GetByUserIdAsync(user.UserId);
                if (buyerProfile == null)
                {
                    _logger.LogError("Not found buyer with Id:{Id}", order.BuyerId);
                }
                var buyer = await _userRepository.GetByIdAsync(order.BuyerId);
                if (buyer == null)
                {
                    _logger.LogError("Not found buyer with Id:{Id}", order.BuyerId);
                }
                var index = new OrderIndexDto()
                {
                    UserName = buyerProfile.FullName,
                    EscrowId = e.EscrowId,
                    Status = EnumHandle.HandleEscrowStatus(e.Status),
                    TotalPrice = e.ActualAmount,
                    UserEmail = buyer.Email,
                    UserId = buyer.UserId,
                };
                result.Indexs.Add(index);
            });
            if (string.IsNullOrEmpty(request.Condition.Text))
            {
                result.Indexs = result.Indexs.Where(x => x.EscrowId.Contains(request.Condition.Text) || x.UserName.Contains(request.Condition.Text)).ToList();
            }
            result.OrderCount = result.Indexs.Count;
            if (request.PageCount > 0 && request.PageSize > 0)
            {
                result.Indexs = result.Indexs.Skip((request.PageCount - 1) * request.PageSize).Take(request.PageSize).ToList();
            }
            return ServiceResult<OrderDto>.Success(result);
        }
        private async Task<ServiceResult<string>> SyncEscrow(IEnumerable<Escrow> escrows)
        {
            var now = DateTime.Today;
            var escrowsPeding = escrows.Where(x => x.Status == EscrowStatus.Pending&& x.Expired >= now ).ToList();
            foreach (var e in escrowsPeding)
            {
                var store = await _storeRepository.GetByIdAsync(e.StoreId);
                if (store == null)
                {
                    _logger.LogError("Not found store with Id:{id}", e.StoreId);
                    return ServiceResult<string> .Error("Không tìm thấy cửa hàng");
                }
                var walletStore = await _walletRepository.GetByUserIdAsync(store.UserId);
                if (walletStore == null)
                {
                    _logger.LogError("Not found wallet of store with Id:{id}", store.UserId);
                    return ServiceResult<string>.Error("không tìm thấy ví người bán");
                }
                var transaction = new WalletTransaction()
                {
                    Amount = e.Amount,
                    Description = $"Hoàn thành kỳ hạn thanh toán cho đơn hàng {e.EscrowId}",
                    Status = WalletTransactionStatus.Completed,
                    Timestamp = DateTime.UtcNow,
                    TransactionId = Guid.NewGuid().ToString(),
                    Type = TransactionType.Deposit,
                    WalletId = walletStore.WalletId,
                };
                var message = new Message()
                {
                    MessageId = Guid.NewGuid().ToString(),
                    Content = "Giải ngân thành công",
                    CreatedAt = DateTime.UtcNow,
                    IsSystem = true,
                    ReceiverId = store.StoreId,
                    ReceiverType = ParticipantType.Store,
                };
                var log = new Log()
                {
                    Action = "Tự động giải ngân ",
                    CreatedAt = DateTime.UtcNow,
                    Description = $"Hoàn tất giao dịch đơn hàng {e.EscrowId}",
                    LogId = Guid.NewGuid().ToString(),
                    TargetId = store.StoreId,
                    TargetType = "Store",
                };
                try
                {
                    e.Status = EscrowStatus.Done;
                    e.UpdatedAt = DateTime.UtcNow;
                    await _ecrowRepository.UpdateAsync(e);
                    await _walletTransactionRepository.AddAsync(transaction);
                    await _messageRepository.AddAsync(message);
                    await _logRepository.AddAsync(log);
                   
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error when update escrow");
                    return ServiceResult<string>.Error("Không thành công");
                }
            }
            return ServiceResult<string>.Success("Thành công");

        }
    }
}
