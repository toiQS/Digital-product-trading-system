using DPTS.Applications.Seller.Dtos.product;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using MediatR;

namespace DPTS.Applications.Seller.Query.product
{
    public class GetProductsWithOptionsQuery : IRequest<ServiceResult<ProductListItemDto>>
    {
        public string Text { get; set; }
        public string CategoryId { get; set; }
        public string SellerId { get; set; }
        public ProductStatus Status { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
    }
}
