using DPTS.Applications.Admin.overview.dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Admin.overview.Queries
{
    public class GetWeeklySalesChartQuery : IRequest<ServiceResult<ChartDto>>
    {
        public string UserId { get; set; }
    }
}
