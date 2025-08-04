using DPTS.Applications.Buyer.Dtos.chat;
using DPTS.Applications.Buyer.Queries.chat;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles.chat
{
    public class GetChatListRecenlyHandler : IRequestHandler<GetChatListRecenlyQuery, ServiceResult<ContactListDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly ILogger<GetChatListRecenlyHandler> _logger;

        public GetChatListRecenlyHandler(IUserRepository userRepository,
                                         IMessageRepository messageRepository,
                                         IStoreRepository storeRepository,
                                         IUserProfileRepository userProfileRepository,
                                         ILogger<GetChatListRecenlyHandler> logger)
        {
            _userRepository = userRepository;
            _messageRepository = messageRepository;
            _storeRepository = storeRepository;
            _userProfileRepository = userProfileRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<ContactListDto>> Handle(GetChatListRecenlyQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetChatListRecenlyQuery for UserId: {UserId}", request.UserId);
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("User not found with UserId :{userid}",request.UserId);
                return ServiceResult<ContactListDto>.Error("Không tìm thấy người dùng");
            }
            var messages = await _messageRepository.GetAllByParticipantAsync(Domains.ParticipantType.Buyer, request.UserId);
            var contacts = new List<string>();
            contacts.AddRange(messages.Select(m => m.SenderId).Distinct());
            contacts.AddRange(messages.Select(m => m.ReceiverId).Distinct());
            contacts = contacts.Where(c => c != request.UserId).Distinct().ToList();
            var result = new ContactListDto
            {
                Contacts = new List<ContactListIndexDto>()
            };
            foreach (var contactId in contacts)
            {
                var contact = await _userRepository.GetByIdAsync(contactId);
                var store = await _storeRepository.GetByUserIdAsync(contactId);
                if (contact != null)
                {
                    var profile = await _userProfileRepository.GetByUserIdAsync(contact.UserId);
                    if (profile == null)
                    {
                        _logger.LogError("User is invalid with UserId: {UserId}", contact.UserId);
                        continue;
                    }
                    result.Contacts.Add(new ContactListIndexDto
                    {
                        Id = contact.UserId,
                        Name = profile.FullName ?? "Error",
                        Avatar = profile.ImageUrl ?? "Error"
                    });
                }
                if (store != null)
                {
                   
                    result.Contacts.Add(new ContactListIndexDto
                    {
                        Id = store.StoreId,
                        Name = store.StoreName ?? "Error",
                        Avatar = store.StoreImage  ?? "Error"
                    });
                }
                if ((contact == null && contact == null) || (store != null && contact != null))
                {
                    _logger.LogError("Found a data is invalid with UserId: {UserId}", contactId);
                    return ServiceResult<ContactListDto>.Error("Dữ liệu không hợp lệ");
                }
            }
            result.Count = result.Contacts.Count;
            return ServiceResult<ContactListDto>.Success(result);
        }
    }
}
