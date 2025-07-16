using DPTS.Applications.Seller.Dtos.revenue;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Seller.Query.revenue
{
    public class GetRevenueChartQuery : IRequest<ServiceResult<IEnumerable<RevenueChartPointDto>>>
    {
        public string SellerId { get; set; } = string.Empty;
        public int CountDay { get; set; } // 7 days - 30 days - 10days
    }
}
