using DPTS.Applications.NoDistinctionOfRoles.products.Dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.NoDistinctionOfRoles.products.Queries
{
    public class GetProductDetailQuery : IRequest<ServiceResult<ProductDetailDto>>
    {
        public string ProductId { get; set; } = string.Empty;
    }
}
