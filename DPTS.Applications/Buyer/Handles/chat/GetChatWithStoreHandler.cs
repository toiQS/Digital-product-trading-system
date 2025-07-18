using DPTS.Applications.Buyer.Dtos;
using DPTS.Applications.Buyer.Dtos.chat;
using DPTS.Applications.Buyer.Queries.chat;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles.chat;

public class GetChatWithStoreHandler : IRequestHandler<GetChatQuery, ServiceResult<ChatIndexListDto>>
{
    private readonly IStoreRepository _storeRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly ILogger<GetChatWithStoreHandler> _logger;

    public GetChatWithStoreHandler(
        IStoreRepository storeRepository,
        IUserProfileRepository userProfileRepository,
        IMessageRepository messageRepository,
        ILogger<GetChatWithStoreHandler> logger)
    {
        _storeRepository = storeRepository;
        _userProfileRepository = userProfileRepository;
        _messageRepository = messageRepository;
        _logger = logger;
    }

    public async Task<ServiceResult<ChatIndexListDto>> Handle(GetChatQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting chat between user {UserId} and store {StoreId}", request.UserId, request.StoreId);

        var result = new ChatIndexListDto();

        // Lấy thông tin người dùng
        var user = await _userProfileRepository.GetByUserIdAsync(request.UserId);
        if (user == null)
        {
            _logger.LogError("User profile not found for userId: {UserId}", request.UserId);
            return ServiceResult<ChatIndexListDto>.Error("Không tìm thấy thông tin người dùng.");
        }

        // Lấy thông tin cửa hàng
        var store = await _storeRepository.GetByIdAsync(request.StoreId);
        if (store == null)
        {
            _logger.LogError("Store not found for storeId: {StoreId}", request.StoreId);
            return ServiceResult<ChatIndexListDto>.Error("Không tìm thấy thông tin cửa hàng.");
        }

        // Lấy toàn bộ tin nhắn trong cuộc trò chuyện
        var conversation = await _messageRepository.GetConversationAsync(
            Domains.ParticipantType.Buyer, request.UserId,
            Domains.ParticipantType.Store, request.StoreId);

        // Gán kết quả trả về
        result.ContentWith = store.StoreName;
        result.Messages = conversation.Select(message => new ChatDto
        {
            Content = message.Content,
            IsOwnMessage = message.SenderId == request.UserId? true: false,
            ReceiveAt = message.CreatedAt
        }).OrderBy(x => x.ReceiveAt).ToList();

        _logger.LogInformation("Retrieved {MessageCount} messages between user {UserId} and store {StoreId}",
            result.Messages.Count, request.UserId, request.StoreId);

        return ServiceResult<ChatIndexListDto>.Success(result);
    }
}
