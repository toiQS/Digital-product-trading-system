using DPTS.Applications.Seller.Dtos.chat;
using DPTS.Applications.Seller.Query.chat;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.Handler.chat
{
    public class SendMessageToBuyerHandler : IRequestHandler<SendMessageToBuyerCommand, ServiceResult<ChatIndexListDto>>
    {
        private readonly IMessageRepository _messageRepository;
        private readonly IUserRepository _userRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly ILogRepository _logRespository;    
        private readonly ILogger<SendMessageToBuyerHandler> _logger;

        public SendMessageToBuyerHandler(IMessageRepository messageRepository, IUserRepository userRepository, IUserProfileRepository userProfileRepository, IStoreRepository storeRepository, ILogRepository logRespository, ILogger<SendMessageToBuyerHandler> logger)
        {
            _messageRepository = messageRepository;
            _userRepository = userRepository;
            _userProfileRepository = userProfileRepository;
            _storeRepository = storeRepository;
            _logRespository = logRespository;
            _logger = logger;
        }

        public async Task<ServiceResult<ChatIndexListDto>> Handle(SendMessageToBuyerCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling SendMessageToBuyerCommand for SellerId: {SellerId}, BuyerId: {BuyerId}", request.SellerId, request.BuyerId);
            var buyer = await _userRepository.GetByIdAsync(request.BuyerId);
            if (buyer == null)
            {
                _logger.LogWarning("Buyer with ID {BuyerId} not found", request.BuyerId);
                return ServiceResult<ChatIndexListDto>.Error("Không tìm thấy người mua");
            }
            var seller = await _userRepository.GetByIdAsync(request.SellerId);
            if(seller == null)
            {
                _logger.LogWarning("Seller with ID {SellerId} not found", request.SellerId);
                return ServiceResult<ChatIndexListDto>.Error("Không tìm thấy người bán");
            }
            var store = await _storeRepository.GetByIdAsync(request.StoreId);
            if (store == null)
            {
                _logger.LogWarning("Store with ID {StoreId} not found", request.StoreId);
                return ServiceResult<ChatIndexListDto>.Error("Không tìm thấy cửa hàng");
            }
            if(store.UserId != seller.UserId)
            {
                _logger.LogWarning("Store with ID {StoreId} does not belong to SellerId: {SellerId}", request.StoreId, request.SellerId);
                return ServiceResult<ChatIndexListDto>.Error("Cửa hàng không thuộc về người bán");
            }

            var message = new Domains.Message
            {
                SenderType = Domains.ParticipantType.Store,
                SenderId = store.StoreId,
                ReceiverType = Domains.ParticipantType.Buyer,
                ReceiverId = request.BuyerId,
                Content = request.Message,
                CreatedAt = DateTime.UtcNow,
                MessageId = Guid.NewGuid().ToString(),
            };
            var log = new Log()
            {
                UserId = request.SellerId,
                Action = "SendMessageToBuyer",
                CreatedAt = DateTime.UtcNow,
                LogId = Guid.NewGuid().ToString(),
                TargetId = request.BuyerId,
                TargetType = "Buyer",
            };
            var buyerProfile = await _userProfileRepository.GetByUserIdAsync(request.BuyerId);
            if (buyerProfile == null)
            {
                _logger.LogWarning("Buyer profile not found for BuyerId: {BuyerId}", request.BuyerId);
                return ServiceResult<ChatIndexListDto>.Error("Không tìm thấy thông tin người mua");
            }
            try
            {
                await _messageRepository.AddAsync(message);
                await _logRespository.AddAsync(log);
                return await GetChatHistory(request, buyerProfile.FullName ??"Error" );
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error sending message from SellerId: {SellerId} to BuyerId: {BuyerId}", request.SellerId, request.BuyerId);
                return ServiceResult<ChatIndexListDto>.Error("Gửi tin nhắn thất bại. Vui lòng thử lại.");
            }
        }
        private async Task<ServiceResult<ChatIndexListDto>> GetChatHistory(SendMessageToBuyerCommand request, string buyerName)
        {
            _logger.LogInformation("Fetching chat history for SellerId: {SellerId}, BuyerId: {BuyerId}", request.SellerId, request.BuyerId);
            var result = new ChatIndexListDto()
            {
                ContentWith = buyerName,
                
            };
            var messages = await _messageRepository.GetConversationAsync(
                Domains.ParticipantType.Store, request.StoreId,
                Domains.ParticipantType.Buyer, request.BuyerId);
            result.Messages = messages.Select(message => new ChatDto
            {
                Content = message.Content,
                IsOwnMessage = message.SenderId == request.StoreId,
                ReceiveAt = message.CreatedAt
            }).OrderBy(x => x.ReceiveAt).ToList();
            return ServiceResult<ChatIndexListDto>.Success(result);
        }
    }
}
