using DPTS.Applications.Admin.manage_revenue.dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Admin.manage_revenue.Queries
{
    public class GetDayOfWeekSalesChartQuery : IRequest<ServiceResult<ChartDto>>
    {
        public string UserId { get; set; }  
    }
}
