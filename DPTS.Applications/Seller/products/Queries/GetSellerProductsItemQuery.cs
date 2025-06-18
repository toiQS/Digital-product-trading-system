using DPTS.Applications.Seller.products.Dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Seller.products.Queries
{
    public class GetSellerProductsItemQuery : IRequest<ServiceResult<IEnumerable<ProductListItemDto>>>
    {
        public string SellerId { get; set; } = string.Empty;
        public int PageSize { get; set; }
        public int PageCount { get; set; }
    }
}
