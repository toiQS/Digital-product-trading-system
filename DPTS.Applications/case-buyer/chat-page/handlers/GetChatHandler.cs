using DPTS.Applications.case_buyer.chat_page.dtos;
using DPTS.Applications.case_buyer.chat_page.models;
using DPTS.Applications.shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repositories.Contracts.Messages;
using DPTS.Infrastructures.Repositories.Contracts.UserProfiles;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.case_buyer.chat_page.handlers
{
    public class GetChatHandler : IRequestHandler<GetChatQuery, ServiceResult<ChatDto>>
    {
        private readonly IMessageQuery _messageQuery;
        private readonly IUserProfileQuery _userProfileQuery;
        private readonly ILogger<GetChatHandler> _logger;

        public GetChatHandler(IMessageQuery messageQuery, IUserProfileQuery userProfileQuery, ILogger<GetChatHandler> logger)
        {
            _messageQuery = messageQuery;
            _userProfileQuery = userProfileQuery;
            _logger = logger;
        }

        public async Task<ServiceResult<ChatDto>> Handle(GetChatQuery request, CancellationToken cancellationToken)
        {
            _logger.LogError("Handing GetChatQuery");
            UserProfile? anotherProfile = await _userProfileQuery.GetByIdAsync(request.AnotherId, cancellationToken);
            if (anotherProfile == null)
            {
                _logger.LogError("Not found anorther persion profile");
                return ServiceResult<ChatDto>.Error("Không tìm thấy người cần trò chuyện");
            }
            var result = new ChatDto();
            result.ChatInfo.Name = anotherProfile.FullName ?? "Error";
            IEnumerable<Message> messages = await _messageQuery.GetsWithIdsJoinChat(ownerId: request.OwnerId, OwnerType: request.OwnerType, anotherId: request.AnotherId, anotherType: request.AnotherType);
            if (!messages.Any())result.MessageDtos = new List<MessageDto>();
            result.MessageDtos = messages.Select(x => new MessageDto()
            {
                Content = x.Content,
                SendAt = x.CreatedAt.ToString("hh:mm")
            }).ToList();
            return ServiceResult<ChatDto>.Success(result);
        }
    }
}
