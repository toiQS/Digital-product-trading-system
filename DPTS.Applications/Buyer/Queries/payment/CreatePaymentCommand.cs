using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.payment
{
    public class CreatePaymentCommand : IRequest<ServiceResult<string>>
    {
        public string OrderId { get; set; }
        public decimal Amount { get; set; }
        public string BankCode { get; set; }
        public string ReturnUrl { get; set; }
    }
}
