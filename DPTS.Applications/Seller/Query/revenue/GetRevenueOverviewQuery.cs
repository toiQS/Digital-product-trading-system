using DPTS.Applications.Seller.Dtos.revenue;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Seller.Query.revenue
{
    public class GetRevenueOverviewQuery: IRequest<ServiceResult<IEnumerable<RevenueOverviewDto>>>
    {
        public string SellerId { get; set; } = string.Empty;
    }
}
