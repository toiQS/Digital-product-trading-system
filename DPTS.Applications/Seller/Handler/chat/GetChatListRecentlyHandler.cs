using DPTS.Applications.Buyer.Queries.chat;
using DPTS.Applications.Seller.Dtos.chat;
using DPTS.Applications.Seller.Query.chat;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.Handler.chat
{
    public class GetChatListRecentlyHandler : IRequestHandler<Query.chat.GetChatListRecentlyQuery, ServiceResult<ContactListDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly ILogger<GetChatListRecentlyHandler> _logger;

        public GetChatListRecentlyHandler(
            IUserRepository userRepository,
            IStoreRepository storeRepository,
            IUserProfileRepository userProfileRepository,
            IMessageRepository messageRepository,
            ILogger<GetChatListRecentlyHandler> logger)
        {
            _userRepository = userRepository;
            _storeRepository = storeRepository;
            _userProfileRepository = userProfileRepository;
            _messageRepository = messageRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<ContactListDto>> Handle(Query.chat.GetChatListRecentlyQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetChatListRecentlyQuery for UserId: {UserId}, StoreId: {StoreId}", request.UserId, request.StoreId);

            // Xác minh người dùng
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("User not found with UserId: {UserId}", request.UserId);
                return ServiceResult<ContactListDto>.Error("Không tìm thấy người dùng");
            }

            var store = await _storeRepository.GetByIdAsync(request.StoreId);
            if (store == null)
            {
                _logger.LogError("Store not found with StoreId: {StoreId}", request.StoreId);
                return ServiceResult<ContactListDto>.Error("Không tìm thấy cửa hàng");
            }

            if (store.UserId != request.UserId)
            {
                _logger.LogError("StoreId: {StoreId} không thuộc về UserId: {UserId}", store.StoreId, request.UserId);
                return ServiceResult<ContactListDto>.Error("Cửa hàng không thuộc về người dùng này");
            }

            
            var messages = await _messageRepository.GetAllByParticipantAsync(Domains.ParticipantType.Store, store.StoreId);
            if (messages == null || !messages.Any())
            {
                _logger.LogInformation("Không tìm thấy message nào cho StoreId: {StoreId}", store.StoreId);
                return ServiceResult<ContactListDto>.Success(new ContactListDto());
            }

            
            var contactIds = messages
                .SelectMany(m => new[] { m.SenderId, m.ReceiverId })
                .Where(id => !string.IsNullOrWhiteSpace(id) && id != store.StoreId)
                .Distinct()
                .ToList();

            if (!contactIds.Any())
            {
                _logger.LogInformation("Không tìm thấy contact nào cho StoreId: {StoreId}", store.StoreId);
                return ServiceResult<ContactListDto>.Success(new ContactListDto());
            }

            
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

            result.Count = result.Contacts.Count;
            return ServiceResult<ContactListDto>.Success(result);
        }
    }
}
