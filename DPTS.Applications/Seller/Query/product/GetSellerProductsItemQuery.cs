using DPTS.Applications.Seller.Dtos.product;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using MediatR;

namespace DPTS.Applications.Seller.Query.product
{
    public class GetSellerProductsItemQuery : IRequest<ServiceResult<ProductListDto>>
    {
        public string SellerId { get; set; } = string.Empty;
        public Condition Condition { get; set; } = new Condition();
        public int PageSize { get; set; }
        public int PageCount { get; set; }
    }
    public class Condition
    {
        public string? Text { get; set; }
        public string? CategoryId { get; set; }
        public ProductStatus Status { get; set; }
    }
}
