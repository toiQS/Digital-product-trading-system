using DPTS.Applications.Buyer.Dtos.order;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.order
{
    public class GetDetailOrderQuery : IRequest<ServiceResult<DetailOrderDto>>
    {
        public string UserId { get; set; } = string.Empty;
        public string EscrowId {  get; set; } = string.Empty;
    }
}
