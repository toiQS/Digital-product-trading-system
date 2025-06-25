using DPTS.Applications.Buyers.payments.Dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyers.payments.Queries
{
    public class GetCheckoutQuery : IRequest<ServiceResult<CheckoutDto>>
    {
        public string BuyerId { get; set; } = string.Empty;
    }
}
