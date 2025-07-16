using DPTS.Applications.Seller.Dtos.product;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Seller.Query.product
{
    public class GetTopSellingProductsQuery : IRequest<ServiceResult<IEnumerable<TopSellingProductDto>>>
    {
        public string SellerId { get; set; } = string.Empty;
    }
}
