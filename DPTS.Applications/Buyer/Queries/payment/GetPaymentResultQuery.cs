using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.payment
{
    public class GetPaymentResultQuery : IRequest<ServiceResult<string>>
    {
        public string UserId { get; set; } = string.Empty;
        public string PaymentMethodId { get; set; } = string.Empty;
    }
}
