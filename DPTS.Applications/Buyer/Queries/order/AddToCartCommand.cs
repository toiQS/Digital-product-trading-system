using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.order
{
    public class AddToCartCommand : IRequest<ServiceResult<string>>
    {
        public string ProjectId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public int Quantities { get; set; }
    }
}
