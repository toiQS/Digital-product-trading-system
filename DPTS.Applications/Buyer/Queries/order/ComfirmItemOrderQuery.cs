using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.order
{
    public class ComfirmItemOrderQuery : IRequest<ServiceResult<string>>
    {
        public string UserId { get; set; } 
        public string EscrowId { get; set; }
    }
}
