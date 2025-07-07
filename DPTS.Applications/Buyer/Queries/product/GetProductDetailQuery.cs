using DPTS.Applications.Buyer.Dtos.product;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.product
{
    public class GetProductDetailQuery  : IRequest<ServiceResult<ProductDetailDto>>
    {
        public string ProjectId { get; set; } = string.Empty;
    }
}
