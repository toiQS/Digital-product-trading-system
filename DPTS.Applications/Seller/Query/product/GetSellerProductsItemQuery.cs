using DPTS.Applications.Seller.Dtos.product;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Seller.Query.product
{
    public class GetSellerProductsItemQuery : IRequest<ServiceResult<IEnumerable<ProductListItemDto>>>
    {
        public string SellerId { get; set; } = string.Empty;
        public int PageSize { get; set; }
        public int PageCount { get; set; }
    }
}
