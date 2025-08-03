using DPTS.Applications.Shareds;
using DPTS.Domains;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.wallet
{
    public class AddPaymentMethodCommand : IRequest<ServiceResult<string>>
    {
        public string UserId { get; set; }
        public AddPaymentCommand AddPaymentCommand { get; set; }
    }
    public class AddPaymentCommand
    {
        public PaymentProvider PaymentProvider { get; set; }
        public string? MaskedAccountNumber { get; set; }
        public bool IsVerified { get; set; }
    }
}
