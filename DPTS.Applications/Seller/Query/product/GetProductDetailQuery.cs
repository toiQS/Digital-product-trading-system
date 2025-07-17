using DPTS.Applications.Seller.Dtos.product;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Seller.Query.product
{
    public class GetProductDetailQuery : IRequest<ServiceResult<ProductDetailDto>>
    {
        public string ProductId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
    }
}
