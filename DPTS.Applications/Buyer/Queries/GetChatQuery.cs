using DPTS.Applications.Buyer.Dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries
{
    public class GetChatQuery : IRequest<ServiceResult<ChatIndexListDto>>
    {
        public string StoreId { get; set; } = string.Empty;
        public string UserId {  get; set; } = string.Empty;
    }
}
