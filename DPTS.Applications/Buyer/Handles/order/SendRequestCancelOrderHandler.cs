using DPTS.Applications.Buyer.Queries.order;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles.order
{
    public class SendRequestCancelOrderHandler : IRequestHandler<SendRequestCancelOrderCommand, ServiceResult<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IEscrowRepository _ecrowRepository;
        private readonly IEscrowProcessRepository _processRepository;
        private readonly ILogRepository _logRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly ILogger<SendRequestCancelOrderHandler> _logger;

        public SendRequestCancelOrderHandler(IUserRepository userRepository,
                                             IOrderRepository orderRepository,
                                             IEscrowRepository ecrowRepository,
                                             IEscrowProcessRepository processRepository,
                                             ILogRepository logRepository,
                                             IMessageRepository messageRepository,
                                             ILogger<SendRequestCancelOrderHandler> logger)
        {
            _userRepository = userRepository;
            _orderRepository = orderRepository;
            _ecrowRepository = ecrowRepository;
            _processRepository = processRepository;
            _logRepository = logRepository;
            _messageRepository = messageRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<string>> Handle(SendRequestCancelOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling send request cancel order");
            var buyer = await _userRepository.GetByIdAsync(request.UserId);
            if (buyer == null)
            {
                _logger.LogError("Not found user with Id: {id}", request.UserId);
                return ServiceResult<string>.Success("Không tìm thấy người dùng người dùng");
            }
            var escrow = await _ecrowRepository.GetByIdAsync(request.Condition.EscrowId);
            if (escrow == null)
            {
                _logger.LogError("Not found escrow with Id: {id}", request.UserId);
                return ServiceResult<string>.Error("Không tìm thấy đơn hàng");
            }
            var order = await _orderRepository.GetByIdAsync(escrow.OrderId);
            if (order == null)
            {
                _logger.LogError("Not found order with Id: {id}", escrow.OrderId);
                return ServiceResult<string>.Error("Không tìm thấy đơn hàng");
            }
            if (order.BuyerId != buyer.UserId)
            {
                _logger.LogError("{buyerId} is not owner of {ecrowId} ",request.UserId, escrow.EscrowId);
                return ServiceResult<string>.Error("Người dùng không sở hữu đơn hàng này");
            }
            var message = new Message()
            {
                IsSystem = true,
                Content = "Hủy đơn hàng",
                CreatedAt = DateTime.UtcNow,
                MessageId = Guid.NewGuid().ToString(),
                ReceiverId = escrow.StoreId,
                ReceiverType = ParticipantType.Store,
            };
            var log = new Log()
            {
                LogId = Guid.NewGuid().ToString(),
                Action = "Người dùng gửi yêu cầu hủy đơn hàng",
                TargetId = escrow.StoreId,
                Description = "Người dùng gửi yêu cầu hủy đơn hàng",
                CreatedAt = DateTime.UtcNow,
                TargetType = "Store",
            };
            
            var process = new EscrowProcess()
            {
                ProcessName = "Người dùng gửi yêu cầu hủy đơn hàng",
                EscrowId = escrow.EscrowId,
                EscrowProcessInformation = "Người dùng gửi yêu cầu hủy đơn hàng",
                ProcessAt = DateTime.UtcNow,
                ProcessId = Guid.NewGuid().ToString(),
            };

            try
            {
                escrow.UpdatedAt = DateTime.UtcNow;
                await _ecrowRepository.UpdateAsync(escrow);
                await _messageRepository.AddAsync(message);
                await _processRepository.AddAsync(process);
                await _logRepository.AddAsync(log);
                return ServiceResult<string>.Success("Gửi yêu cầu hủy đơn hàng thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, " Error when send request cancel order");
                return ServiceResult<string>.Error("Gửi yêu cầu gửi đơn hàng không thành công");
            }
        }
    }
}
