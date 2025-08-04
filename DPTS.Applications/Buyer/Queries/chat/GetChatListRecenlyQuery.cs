using DPTS.Applications.Buyer.Dtos.chat;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.chat
{
    public class GetChatListRecenlyQuery : IRequest<ServiceResult<ContactListDto>>
    {
        public string UserId { get; set; }
    }
}
