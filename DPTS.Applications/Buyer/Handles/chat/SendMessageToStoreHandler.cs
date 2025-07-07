using DPTS.Applications.Buyer.Dtos;
using DPTS.Applications.Buyer.Dtos.chat;
using DPTS.Applications.Buyer.Queries.chat;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles.chat;

public class SendMessageToStoreHandler : IRequestHandler<SendMessageToStoreQuery, ServiceResult<ChatIndexListDto>>
{
    private readonly IMessageRepository _messageRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly ILogRepository _logRepository;
    private readonly ILogger<SendMessageToStoreHandler> _logger;

    public SendMessageToStoreHandler(
        IMessageRepository messageRepository,
        IUserProfileRepository userProfileRepository,
        IStoreRepository storeRepository,
        ILogRepository logRepository,
        ILogger<SendMessageToStoreHandler> logger)
    {
        _messageRepository = messageRepository;
        _userProfileRepository = userProfileRepository;
        _storeRepository = storeRepository;
        _logRepository = logRepository;
        _logger = logger;
    }

    public async Task<ServiceResult<ChatIndexListDto>> Handle(SendMessageToStoreQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Sending message from user {UserId} to store {StoreId}", request.UserId, request.StoreId);

        // Kiểm tra thông tin người dùng
        var user = await _userProfileRepository.GetByUserIdAsync(request.UserId);
        if (user == null)
        {
            _logger.LogError("User profile not found for userId: {UserId}", request.UserId);
            return ServiceResult<ChatIndexListDto>.Error("Không tìm thấy thông tin người dùng.");
        }

        // Kiểm tra thông tin cửa hàng
        var store = await _storeRepository.GetByIdAsync(request.StoreId);
        if (store == null)
        {
            _logger.LogError("Store not found for storeId: {StoreId}", request.StoreId);
            return ServiceResult<ChatIndexListDto>.Error("Không tìm thấy thông tin cửa hàng.");
        }

        // Tạo đối tượng tin nhắn
        var message = new Message
        {
            MessageId = Guid.NewGuid().ToString(),
            Content = request.Content,
            CreatedAt = DateTime.UtcNow,
            SenderId = request.UserId,
            SenderType = ParticipantType.User,
            ReceiverId = request.StoreId,
            ReceiverType = ParticipantType.Store,
            IsSystem = false
        };

        // Ghi log gửi tin nhắn
        var log = new Log
        {
            LogId = Guid.NewGuid().ToString(),
            UserId = request.UserId,
            Action = "SendMessageToStore",
            TargetId = store.StoreId,
            TargetType = "Store",
            CreatedAt = DateTime.UtcNow,
        };

        try
        {
            await _messageRepository.AddAsync(message);
            await _logRepository.AddAsync(log);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to send message from user {UserId} to store {StoreId}", request.UserId, request.StoreId);
            return ServiceResult<ChatIndexListDto>.Error("Gửi tin nhắn thất bại. Vui lòng thử lại.");
        }

        // Sau khi gửi thành công → trả lại toàn bộ cuộc hội thoại cập nhật mới nhất
        return await GetChatHistory(request, store.StoreName);
    }

    /// <summary>
    /// Lấy toàn bộ lịch sử chat giữa user và store sau khi gửi tin nhắn.
    /// </summary>
    private async Task<ServiceResult<ChatIndexListDto>> GetChatHistory(SendMessageToStoreQuery request, string storeName)
    {
        _logger.LogInformation("Retrieving conversation between user {UserId} and store {StoreId}", request.UserId, request.StoreId);

        var result = new ChatIndexListDto
        {
            ContentWith = storeName
        };

        var conversation = await _messageRepository.GetConversationAsync(
            ParticipantType.User, request.UserId,
            ParticipantType.Store, request.StoreId);

        result.Messages = conversation
            .OrderBy(x => x.CreatedAt)
            .Select(message => new ChatDto
            {
                Content = message.Content,
                IsOwnMessage = message.SenderId == request.UserId,
                ReceiveAt = message.CreatedAt
            }).ToList();

        _logger.LogInformation("Conversation loaded with {MessageCount} messages", result.Messages.Count);
        return ServiceResult<ChatIndexListDto>.Success(result);
    }
}
