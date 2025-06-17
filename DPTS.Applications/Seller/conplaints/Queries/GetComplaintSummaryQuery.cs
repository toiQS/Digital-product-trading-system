using DPTS.Applications.Seller.conplaints.Dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Seller.conplaints.Queries
{
    public class GetComplaintSummaryQuery : IRequest<ServiceResult<IEnumerable<ComplaintSummaryDto>>>
    {
        public string SellerId { get; set; } = string.Empty;
    }
}
