using DPTS.Applications.Admin.manage_product.dtos;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using MediatR;

namespace DPTS.Applications.Admin.manage_product.Queries
{
    public class GetProductsQuery : IRequest<ServiceResult<ProductDto>>
    {
        public string UserId { get; set; }
        public Condition Condition { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
    public class Condition
    {
        public string? Text { get; set; }
        public string? CategoryId{ get; set; }
        public ProductStatus ProductStatus { get; set; }
    }
}
