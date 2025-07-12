using DPTS.Applications.Sellers.overviews.Dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Sellers.overviews.Queries
{
    public class GetSellerOverviewQuery : IRequest<ServiceResult<IEnumerable<SellerOverviewDto>>>
    {
        public string SellerId {  get; set; } = string.Empty;
        public int CountDay { get; set; } //  7d = 1 week, 30day = month
    }
}
