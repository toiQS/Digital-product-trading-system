using DPTS.Applications.Admin.overview.dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Admin.overview.Queries
{
    public class GetOverviewQuery : IRequest<ServiceResult<IEnumerable<OverviewIndexDto>>>
    {
        public string UserId { get; set; }
    }
}
