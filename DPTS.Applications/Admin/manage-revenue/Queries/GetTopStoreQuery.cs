using DPTS.Applications.Admin.manage_revenue.dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Admin.manage_revenue.Queries
{
    public class GetTopStoreQuery : IRequest<ServiceResult<TopStoreDto>>
    {
        public string UserId { get; set; }
    }
}
