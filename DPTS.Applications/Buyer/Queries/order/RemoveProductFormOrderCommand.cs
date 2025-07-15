using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.order
{
    public class RemoveProductFormOrderCommand : IRequest<ServiceResult<string>>
    {
        public string UserId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        
    }
}
