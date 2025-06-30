using DPTS.Applications.Buyer.Dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries
{
    public class GetProductDetailQuery  : IRequest<ServiceResult<ProductDetailDto>>
    {
        public string ProjectId { get; set; } = string.Empty;
    }
}
