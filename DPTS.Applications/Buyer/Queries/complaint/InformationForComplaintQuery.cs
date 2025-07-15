using DPTS.Applications.Buyer.Dtos.complaint;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.complaint
{
    public class InformationForComplaintQuery : IRequest<ServiceResult<InformationForComplaintDto>>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
