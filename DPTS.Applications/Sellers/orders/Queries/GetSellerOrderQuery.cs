using DPTS.Applications.Sellers.orders.Dtos;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using MediatR;

namespace DPTS.Applications.Sellers.orders.Queries
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
