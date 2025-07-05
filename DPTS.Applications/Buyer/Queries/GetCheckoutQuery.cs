using DPTS.Applications.Buyer.Dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries
{
    public class GetCheckoutQuery : IRequest<ServiceResult<CheckoutDto>>
    {
        public string BuyerId { get; set; } = string.Empty;
    }
}
