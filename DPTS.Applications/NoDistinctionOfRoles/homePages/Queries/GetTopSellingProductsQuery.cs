using DPTS.Applications.NoDistinctionOfRoles.homePages.Dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.NoDistinctionOfRoles.homePages.Queries
{
    public class GetTopSellingProductsQuery : IRequest<ServiceResult<IEnumerable<TopSellingProductDto>>>
    {
    }
}
