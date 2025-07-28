using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.order
{
    public class SendRequestCancelOrderCommand : IRequest<ServiceResult<string>>
    {
        public string UserId { get; set; }
        public Condition Condition { get; set; }
    }
    public class Condition
    {
        public string EscrowId { get; set; }
    }
}
