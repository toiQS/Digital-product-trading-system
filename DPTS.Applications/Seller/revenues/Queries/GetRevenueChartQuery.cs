using DPTS.Applications.Seller.revenues.Dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Seller.revenues.Queries
{
    public class GetRevenueChartQuery : IRequest<ServiceResult<IEnumerable<RevenueChartPointDto>>>
    {
        public string SellerId { get; set; } = string.Empty;
        public int CountDay { get; set; } // 7 days - 30 days - 10days
    }
}
