using DPTS.Applications.Admin.overview.dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Admin.overview.Queries
{
    internal class GetTopStoreQuery : IRequest<ServiceResult<TopStoreDto>>
    {
        public string UserId { get; set; }
    }
}
