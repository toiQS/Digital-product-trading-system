using DPTS.Applications.Seller.revenues.Dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Seller.revenues.Queries
{
    public class GetRevenueOverviewQuery: IRequest<ServiceResult<IEnumerable<RevenueOverviewDto>>>
    {
        public string SellerId { get; set; } = string.Empty;
    }
}
