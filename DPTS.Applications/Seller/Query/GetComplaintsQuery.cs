using DPTS.Applications.Seller.conplaints.Dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Seller.conplaints.Queries
{
    public class GetComplaintsQuery : IRequest<ServiceResult<IEnumerable<ComplaintListItemDto>>>
    {
        public string SellerId { get; set; } = string.Empty;
    }
}
