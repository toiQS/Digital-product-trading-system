using DPTS.Applications.Buyer.Dtos.order;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.order
{
    public class GetCheckoutQuery : IRequest<ServiceResult<CheckoutDto>>
    {
        public string BuyerId { get; set; } = string.Empty;
    }
}
