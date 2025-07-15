using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.complaint
{
    public class AddComplaintCommand : IRequest<ServiceResult<string>>
    {
        public string UserId { get; set; } = string.Empty;
        public string ItemId { get; set; } = string.Empty;
        public string ComplaintType { get; set; } = string.Empty;
        public string Description {  get; set; } = string.Empty;
        public List<string> ComplaintImages { get; set; }
        public string MethodContact {  get; set; } = string.Empty;
    }
}
