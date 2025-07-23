using DPTS.Applications.Admin.manage_user.dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Admin.manage_user.Queries
{
    public class GetOverviewQuery : IRequest<ServiceResult<IEnumerable<OverviewIndexDto>>>  
    {
        public string UserId { get; set; }
    }
}
