using DPTS.Applications.Sellers.conplaints.Dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Sellers.conplaints.Queries
{
    public class GetComplaintsQuery : IRequest<ServiceResult<IEnumerable<ComplaintListItemDto>>>
    {
        public string SellerId { get; set; } = string.Empty;
    }
}
