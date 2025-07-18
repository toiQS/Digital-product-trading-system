using DPTS.Applications.Seller.Dtos.chat;
using DPTS.Applications.Seller.Query.chat;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.Handler.chat
{
    public class GetChatHandler : IRequestHandler<GetChatQuery, ServiceResult<ChatIndexListDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly ILogger<GetChatHandler> _logger;
        private readonly IUserProfileRepository _userProfileRepository;

        public GetChatHandler(IUserRepository userRepository,
                              IMessageRepository messageRepository,
                              IStoreRepository storeRepository,
                              ILogger<GetChatHandler> logger,
                              IUserProfileRepository userProfileRepository)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _storeRepository = storeRepository;
            _logger = logger;
            _userProfileRepository = userProfileRepository;
        }

        public async Task<ServiceResult<ChatIndexListDto>> Handle(GetChatQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetChatQuery for BuyerId: {BuyerId}, StoreId: {StoreId}, SellerId: {SellerId}",
                               request.BuyerId, request.StoreId, request.SellerId);
            var buyer = await _userRepository.GetByIdAsync(request.BuyerId);
            if (buyer == null)
            {
                _logger.LogWarning("Buyer with ID {BuyerId} not found", request.BuyerId);
                return ServiceResult<ChatIndexListDto>.Error("Không tìm thấy người mua");
            }
            var seller = await _userRepository.GetByIdAsync(request.SellerId);
            if (seller == null)
            {
                _logger.LogWarning("Seller with ID {SellerId} not found", request.SellerId);
                return ServiceResult<ChatIndexListDto>.Error("Không tìm thấy người bán");
            }
            var store = await _storeRepository.GetByIdAsync(request.StoreId);
            if( store == null)
            {
                _logger.LogWarning("Store with ID {StoreId} not found", request.StoreId);
                return ServiceResult<ChatIndexListDto>.Error("Không tìm thấy cửa hàng");
            }
            if(seller.UserId != store.UserId)
            {
                _logger.LogWarning("Seller with ID {SellerId} does not own Store with ID {StoreId}", request.SellerId, request.StoreId);
                return ServiceResult<ChatIndexListDto>.Error("Người bán không sở hữu cửa hàng này");
            }
            var  messages = await _messageRepository.GetConversationAsync(Domains.ParticipantType.Buyer, request.BuyerId,
                Domains.ParticipantType.Store, request.StoreId);
            var buyerProfile = await _userProfileRepository.GetByUserIdAsync(request.BuyerId);
            if (buyerProfile == null)
            {
                _logger.LogWarning("Buyer profile with UserId {UserId} not found", request.BuyerId);
                return ServiceResult<ChatIndexListDto>.Error("Không tìm thấy thông tin người mua");
            }
            var dto = new ChatIndexListDto()
            {
                ContentWith = buyerProfile.FullName??"Error",
                Messages = messages.Select(m => new ChatDto
                {
                    Content = m.Content,
                    IsOwnMessage = (m.SenderId == request.StoreId)?true : false,
                    ReceiveAt = m.CreatedAt,
                }).OrderByDescending(x => x.ReceiveAt).ToList()
            };
            return ServiceResult<ChatIndexListDto>.Success(dto);
        }
    }
}
