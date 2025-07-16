using DPTS.Applications.Seller.Dtos.complaint;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Seller.Query.complaint
{
    public class GetComplaintOverviewQuery : IRequest<ServiceResult<IEnumerable<ComplaintOverviewDto>>>
    {
        public string SellerId { get; set; } = string.Empty;
    }
}
