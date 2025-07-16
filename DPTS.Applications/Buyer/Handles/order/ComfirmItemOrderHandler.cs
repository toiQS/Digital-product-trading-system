using DPTS.Applications.Buyer.Queries.order;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles.order
{
    public class ComfirmItemOrderHandler : IRequestHandler<ComfirmItemOrderQuery, ServiceResult<string>>
    {
        private readonly IEscrowRepository _escrowRepository;
        private readonly IEscrowProcessRepository _escrowProcessRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<ComfirmItemOrderHandler> _logger;
        private readonly ILogRepository _logRepository;
        private readonly IOrderPaymentRepository _orderPaymentRepository;
        private readonly IStoreRepository _storeRepository;

        public ComfirmItemOrderHandler(IEscrowRepository escrowRepository,
                                       IEscrowProcessRepository escrowProcessRepository,
                                       IMessageRepository messageRepository,
                                       IWalletTransactionRepository walletTransactionRepository,
                                       IWalletRepository walletRepository,
                                       IUserProfileRepository userProfileRepository,
                                       IOrderRepository orderRepository,
                                       ILogger<ComfirmItemOrderHandler> logger,
                                       ILogRepository logRepository,
                                       IOrderPaymentRepository orderPaymentRepository,
                                       IStoreRepository storeRepository)
        {
            _escrowRepository = escrowRepository;
            _escrowProcessRepository = escrowProcessRepository;
            _messageRepository = messageRepository;
            _walletTransactionRepository = walletTransactionRepository;
            _walletRepository = walletRepository;
            _userProfileRepository = userProfileRepository;
            _orderRepository = orderRepository;
            _logger = logger;
            _logRepository = logRepository;
            _orderPaymentRepository = orderPaymentRepository;
            _storeRepository = storeRepository;
        }

        public async Task<ServiceResult<string>> Handle(ComfirmItemOrderQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling ComfirmItemOrderQuery for UserId={UserId}, EscrowId={EscrowId}", request.UserId, request.EscrowId);
            var user = await _userProfileRepository.GetByUserIdAsync(request.UserId);
            if(user == null)
            {
                _logger.LogError("User not found. UserId={UserId}", request.UserId);
                return ServiceResult<string>.Error("Không tìm thấy người dùng.");
            }
            var escrow = await _escrowRepository.GetByIdAsync(request.EscrowId);
            if(escrow == null)
            {
                _logger.LogError("Escrow not found. EscrowId={EscrowId}", request.EscrowId);
                return ServiceResult<string>.Error("Không tìm thấy giao dịch.");
            }
            var order = await _orderRepository.GetByIdAsync(escrow.OrderId);
            if(order == null)
            {
                _logger.LogError("Order not found. OrderId={OrderId}", escrow.OrderId);
                return ServiceResult<string>.Error("Không tìm thấy đơn hàng.");
            }
            var store = await _storeRepository.GetByIdAsync(escrow.StoreId);
            if(store == null)
            {
                _logger.LogError("Store not found. StoreId={StoreId}", escrow.StoreId);
                return ServiceResult<string>.Error("Không tìm thấy cửa hàng.");
            }
            var seller = await _userProfileRepository.GetByUserIdAsync(store.UserId);
            if(seller == null)
            {
                _logger.LogError("Seller not found. UserId={UserId}", store.UserId);
                return ServiceResult<string>.Error("Không tìm thấy người bán.");
            }
            var walletSeller = await _walletRepository.GetByUserIdAsync(store.UserId);
            if(walletSeller == null)
            {
                _logger.LogError("Wallet not found for seller. UserId={UserId}", store.UserId);
                return ServiceResult<string>.Error("Không tìm thấy ví của người bán.");
            }
            if (order.BuyerId != user.UserId)
            {
                _logger.LogError("Order does not belong to user. UserId={UserId}, OrderId={OrderId}", request.UserId, order.OrderId);
                return ServiceResult<string>.Error("Đơn hàng không thuộc về người dùng này.");
            }
            
            var orderPayment = await _orderPaymentRepository.GetByOrderIdAsync(order.OrderId);
            if(orderPayment == null)
            {
                _logger.LogError("Order payment not found. OrderId={OrderId}", order.OrderId);
                return ServiceResult<string>.Error("Không tìm thấy thông tin thanh toán cho đơn hàng.");
            }
            
            var transaction = new WalletTransaction()
            {
                TransactionId = Guid.NewGuid().ToString(),
                Amount = escrow.Amount,
                Description = $"Xác nhận giao dịch {escrow.EscrowId} cho đơn hàng {order.OrderId}",
                Status = WalletTransactionStatus.Completed,
                Type = TransactionType.Purchase,
                Timestamp = DateTime.UtcNow,
                WalletId = walletSeller.WalletId,
                LinkedPaymentMethod = orderPayment.SourceType == PaymentSourceType.PaymentMethod ? 
                    await _walletTransactionRepository.GetPaymentMethodAsync(orderPayment.PaymentMethodId) : null
            };
            var mesage = new Message()
            {
                MessageId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                ReceiverType = ParticipantType.Store,
                ReceiverId = seller.UserId,
                IsSystem = true,
                Content = $"Giao dịch {escrow.EscrowId} đã được xác nhận hoàn thành bởi người mua.",
            };
            var log = new Log()
            {
                LogId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                UserId = user.UserId,
                Action = "ConfirmOrderItem",
                TargetId = seller.UserId,
                TargetType = "Store",
            };
            var escrowProcess = new EscrowProcess()
            {
                ProcessId = Guid.NewGuid().ToString(),
                EscrowId = escrow.EscrowId,
                EscrowProcessInformation = $"Giao dịch {escrow.EscrowId} đã được xác nhận hoàn thành bởi người mua.",
                ProcessAt = DateTime.Now,
            };
            try
            {
                escrow.Status = Domains.EscrowStatus.Done;

                await _escrowRepository.UpdateAsync(escrow);
                await _walletTransactionRepository.AddAsync(transaction);
                await _messageRepository.AddAsync(mesage);
                await _logRepository.AddAsync(log);
                await _escrowProcessRepository.AddAsync(escrowProcess);
                _logger.LogInformation("Transaction completed for EscrowId={EscrowId}, OrderId={OrderId}", escrow.EscrowId, order.OrderId);
                return ServiceResult<string>.Success("Giao dịch đã hoàn thành.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while processing transaction for EscrowId={EscrowId}", escrow.EscrowId);
                return ServiceResult<string>.Error("Có lỗi xảy ra trong quá trình xử lý giao dịch.");
            }
        }
    }
}
