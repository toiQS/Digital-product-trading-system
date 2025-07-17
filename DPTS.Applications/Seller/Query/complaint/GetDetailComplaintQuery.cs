using DPTS.Applications.Seller.Dtos.complaint;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Seller.Query.complaint
{
    public class GetDetailComplaintQuery : IRequest<ServiceResult<GetDetailComplaintDto>>
    {
        public string ComplaintId { get; set; } = string.Empty;
        public string SellerId { get; set; } = string.Empty;
    }
}
