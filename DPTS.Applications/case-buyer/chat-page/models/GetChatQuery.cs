using DPTS.Applications.case_buyer.chat_page.dtos;
using DPTS.Applications.shareds;
using DPTS.Domains;
using MediatR;

namespace DPTS.Applications.case_buyer.chat_page.models
{
    public record GetChatQuery : IRequest<ServiceResult<ChatDto>>
    {
        public string OwnerId { get; set; }
        public ParticipantType OwnerType { get; set; }
        public string AnotherId { get; set; }
        public ParticipantType AnotherType {  get; set; }
    }
}
