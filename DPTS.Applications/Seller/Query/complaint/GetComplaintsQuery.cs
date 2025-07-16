using DPTS.Applications.Seller.Dtos.complaint;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Seller.Query.complaint
{
    public class GetComplaintsQuery : IRequest<ServiceResult<IEnumerable<ComplaintListItemDto>>>
    {
        public string SellerId { get; set; } = string.Empty;
    }
}
