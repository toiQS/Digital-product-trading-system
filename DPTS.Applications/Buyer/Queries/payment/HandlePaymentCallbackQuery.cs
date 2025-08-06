using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.payment
{
    public class HandlePaymentCallbackQuery : IRequest<ServiceResult<string>>
    {
        public Dictionary<string, string> QueryData { get; set; }
    }
}
