using DPTS.Applications.Admin.overview.dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Admin.overview.Queries
{
    public class GetUserActivityQuery : IRequest<ServiceResult<UserActivityDto>>
    {
        public string UserId { get; set; }
    }
}
