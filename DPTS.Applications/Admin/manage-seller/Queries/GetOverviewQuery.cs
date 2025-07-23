using DPTS.Applications.Admin.manage_seller.dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Admin.manage_seller.Queries
{
    public class GetOverviewQuery  : IRequest<ServiceResult<OverviewDto>>
    {
        public string UserId { get; set; }
    }
}
