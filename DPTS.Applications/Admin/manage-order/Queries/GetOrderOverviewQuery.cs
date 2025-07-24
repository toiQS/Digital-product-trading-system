using DPTS.Applications.Admin.manage_order.dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Admin.manage_order.Queries
{
    public class GetOrderOverviewQuery : IRequest<ServiceResult<OrderOverviewDto>>
    {
        public string UserId { get; set; }
    }
}
