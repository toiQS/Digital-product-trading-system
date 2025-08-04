using DPTS.Applications.Seller.Dtos.chat;
using DPTS.Applications.Seller.Query.chat;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Implements;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.Handler.chat
{
    public class GetChatListRecenlyHandler : IRequestHandler<GetChatListRecenlyQuery, ServiceResult<ContactListDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly ILogger<GetChatListRecenlyHandler> _logger;

        public GetChatListRecenlyHandler(IUserRepository userRepository, IStoreRepository storeRepository, IUserProfileRepository userProfileRepository, IMessageRepository messageRepository, ILogger<GetChatListRecenlyHandler> logger)
        {
            _userRepository = userRepository;
            _storeRepository = storeRepository;
            _userProfileRepository = userProfileRepository;
            _messageRepository = messageRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<ContactListDto>> Handle(GetChatListRecenlyQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetChatListRecenlyQuery for UserId: {UserId}, StoreId: {StoreId}", request.UserId, request.StoreId);
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if(user == null)
            {
                _logger.LogError("User not found with UserId :{userid}", request.UserId);
                return ServiceResult<ContactListDto>.Error("Không tìm thấy người dùng");
            }
            var store = await _storeRepository.GetByIdAsync(request.StoreId);
            if (store == null)
            {
                _logger.LogError("Store not found with StoreId :{storeid}", request.StoreId);
                return ServiceResult<ContactListDto>.Error("Không tìm thấy cửa hàng");
            }
            if (store.UserId != request.UserId)
            {
                _logger.LogError("StoreId: {StoreId} does not match UserId: {UserId}", store.UserId, request.UserId);
                return ServiceResult<ContactListDto>.Error("Cửa hàng không thuộc về người dùng này");
            }
            var messages = await _messageRepository.GetAllByParticipantAsync(Domains.ParticipantType.Buyer, request.UserId);
            var contacts = new List<string>();
            contacts.AddRange(messages.Select(m => m.SenderId).Distinct());
            contacts.AddRange(messages.Select(m => m.ReceiverId).Distinct());
            contacts = contacts.Where(c => c != request.StoreId).Distinct().ToList();
            var result = new ContactListDto
            {
                Contacts = new List<ContactListIndexDto>()
            };
            foreach (var contactId in contacts)
            {
                var contact = await _userRepository.GetByIdAsync(contactId);
                var storeOther = await _storeRepository.GetByUserIdAsync(contactId);
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
                if (storeOther != null)
                {

                    result.Contacts.Add(new ContactListIndexDto
                    {
                        Id = storeOther.StoreId,
                        Name = storeOther.StoreName ?? "Error",
                        Avatar = storeOther.StoreImage ?? "Error"
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
