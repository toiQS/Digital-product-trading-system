using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries
{
    public class AddToCartQuery : IRequest<ServiceResult<string>>
    {
        public string ProjectId { get; set; } = string.Empty;
        public string UsertId { get; set; } = string.Empty;
        public int Quantities { get; set; }
    }
}
