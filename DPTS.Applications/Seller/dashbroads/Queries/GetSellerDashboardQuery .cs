using DPTS.Applications.Seller.dashbroads.Dtos;
using MediatR;

namespace DPTS.Applications.Seller.dashbroads.Queries
{
    public class GetSellerDashboardQuery : IRequest<SellerDashboardDto>
    {
        public string SellerId { get; set; } = string.Empty;
        public GetSellerDashboardQuery(string selelrId)
        {
            SellerId = selelrId;
        }
    }
}
