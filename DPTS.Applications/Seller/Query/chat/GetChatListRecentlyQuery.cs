using DPTS.Applications.Seller.Dtos.chat;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Seller.Query.chat
{
    public class GetChatListRecentlyQuery : IRequest<ServiceResult<ContactListDto>>
    {
        public string UserId { get; set; } = string.Empty;
        public string StoreId { get; set; } = string.Empty;
    }
}
