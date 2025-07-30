using DPTS.Applications.Buyer.Dtos.order;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.order
{
    public class GetPurchasedOrderQuery : IRequest<ServiceResult<PurchasedOrderDto>>
    {
        public string UserId { get; set; } = string.Empty;
        public string? Text { get; set; } = string.Empty;
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
