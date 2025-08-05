using DPTS.Applications.Buyer.Dtos.chat;
using DPTS.Applications.Buyer.Queries.chat;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handlers.chat
{
    public class GetChatListRecentlyHandler : IRequestHandler<GetChatListRecentlyQuery, ServiceResult<ContactListDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly ILogger<GetChatListRecentlyHandler> _logger;

        public GetChatListRecentlyHandler(IUserRepository userRepository,
                                          IMessageRepository messageRepository,
                                          IStoreRepository storeRepository,
                                          IUserProfileRepository userProfileRepository,
                                          ILogger<GetChatListRecentlyHandler> logger)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _storeRepository = storeRepository;
            _userProfileRepository = userProfileRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<ContactListDto>> Handle(GetChatListRecentlyQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetChatListRecentlyQuery for UserId: {UserId}", request.UserId);

            
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("User not found with UserId: {UserId}", request.UserId);
                return ServiceResult<ContactListDto>.Error("Không tìm thấy người dùng");
            }

            
            var messages = await _messageRepository.GetAllByParticipantAsync(Domains.ParticipantType.Buyer, request.UserId);
            if (messages == null || !messages.Any())
            {
                _logger.LogInformation("Không tìm thấy message nào cho Buyer UserId: {UserId}", request.UserId);
                return ServiceResult<ContactListDto>.Success(new ContactListDto());
            }

            
            var contactIds = messages
                .SelectMany(m => new[] { m.SenderId, m.ReceiverId })
                .Where(id => !string.IsNullOrWhiteSpace(id) && id != request.UserId)
                .Distinct()
                .ToList();

            if (!contactIds.Any())
            {
                _logger.LogInformation("Không tìm thấy contact nào cho Buyer UserId: {UserId}", request.UserId);
                return ServiceResult<ContactListDto>.Success(new ContactListDto());
            }

            
            var stores = await _storeRepository.GetByIdsAsync(contactIds);
            var users = await _userRepository.GetByIdsAsync(contactIds);
            var profiles = await _userProfileRepository.GetByUserIdsAsync(users.Select(u => u.UserId).ToList());

            var result = new ContactListDto();

            foreach (var contact in users)
            {
                var profile = profiles.FirstOrDefault(p => p.UserId == contact.UserId);
                if (profile == null)
                {
                    _logger.LogWarning("Không tìm thấy profile cho UserId: {UserId}", contact.UserId);
                    continue;
                }

                result.Contacts.Add(new ContactListIndexDto
                {
                    Id = contact.UserId,
                    Name = profile.FullName ?? "Không rõ",
                    Avatar = profile.ImageUrl ?? ""
                });
            }

            foreach (var store in stores)
            {
                result.Contacts.Add(new ContactListIndexDto
                {
                    Id = store.StoreId,
                    Name = store.StoreName ?? "Không rõ",
                    Avatar = store.StoreImage ?? ""
                });
            }

            result.Count = result.Contacts.Count;
            return ServiceResult<ContactListDto>.Success(result);
        }
    }
}
