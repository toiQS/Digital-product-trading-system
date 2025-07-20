using DPTS.Applications.case_buyer.chat_page.dtos;
using DPTS.Applications.shareds;
using DPTS.Domains;
using MediatR;

namespace DPTS.Applications.case_buyer.chat_page.models
{
    public class SendMessageCommand : IRequest<ServiceResult<ChatDto>>
    {
        public string PersonFirstId { get; set; }
        public ParticipantType PersonFirstType { get; set; }
        public string PersonSecondId { get; set; }
        public ParticipantType PersonSecondType { get; set; }
        public string Content { get; set; }
    }
}
