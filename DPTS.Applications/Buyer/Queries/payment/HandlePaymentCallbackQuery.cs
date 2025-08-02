using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.payment
{
    public class HandlePaymentCallbackQuery : IRequest<ServiceResult<string>>
    {
        public string vnp_TxnRef { get; set; }
        public string vnp_ResponseCode { get; set; }
        public string vnp_SecureHash { get; set; }

       
    }
}
