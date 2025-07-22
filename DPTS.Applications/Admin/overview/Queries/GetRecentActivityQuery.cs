using DPTS.Applications.Admin.overview.dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Admin.overview.Queries
{
    public class GetRecentActivityQuery : IRequest<ServiceResult<IEnumerable<RecentlyActivityDto>>>
    {
        public string UserId { get; set; }
    }
}
