using DPTS.Applications.Admin.manage_revenue.dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Admin.manage_revenue.Queries
{
    public class GetRevenueAnalysisQuery : IRequest<ServiceResult<RevenueAnalysisDto>>
    {
        public string UserId { get; set; }
    }
}
