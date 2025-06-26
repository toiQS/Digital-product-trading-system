using DPTS.Applications.Buyers.payments.Dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyers.payments.Queries
{
    public class GetPaymentResultQuery : IRequest<ServiceResult<PaymentResultDto>>
    {
        public string BuyerId { get; set; } = string.Empty;
    }
}
