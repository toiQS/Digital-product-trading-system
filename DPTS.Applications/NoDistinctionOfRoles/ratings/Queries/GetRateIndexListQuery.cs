using DPTS.Applications.NoDistinctionOfRoles.ratings.Dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.NoDistinctionOfRoles.ratings.Queries
{
    public class GetRateIndexListQuery : IRequest<ServiceResult<IEnumerable<RateIndexListDto>>>
    {
    }
}
