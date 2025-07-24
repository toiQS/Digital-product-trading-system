using DPTS.Applications.Admin.manage_product.dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Admin.manage_product.Queries
{
    public class GetOverviewQuery : IRequest<ServiceResult<OverviewDto>>
    {
        public string UserId { get; set; }
    }
}
