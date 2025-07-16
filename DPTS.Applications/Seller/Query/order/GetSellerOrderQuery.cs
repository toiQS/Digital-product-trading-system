using DPTS.Applications.Seller.Dtos.order;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using MediatR;

namespace DPTS.Applications.Seller.Query.order
{
    public class GetSellerOrderQuery : IRequest<ServiceResult<IEnumerable<OrderListItemDto>>>
    {
        public string SellerId { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public EscrowStatus Status { get; set; }
        public int PageSize { get; set; }
        public int PageCount { get; set; }
    }
}
