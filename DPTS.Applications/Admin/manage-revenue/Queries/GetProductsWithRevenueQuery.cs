using DPTS.Applications.Admin.manage_revenue.dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Admin.manage_revenue.Queries
{
    public class GetProductsWithRevenueQuery : IRequest<ServiceResult<ProductWithRevenueDto>>
    {
        public string UserId { get; set; }
    }
}
