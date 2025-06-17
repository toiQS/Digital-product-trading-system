using DPTS.Applications.Seller.revenues.Dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Seller.revenues.Queries
{
    public class GetTopSellingProductsQuery : IRequest<ServiceResult<IEnumerable<TopSellingProductDto>>>
    {
        public string SellerId { get; set; } = string.Empty;
    }
}
