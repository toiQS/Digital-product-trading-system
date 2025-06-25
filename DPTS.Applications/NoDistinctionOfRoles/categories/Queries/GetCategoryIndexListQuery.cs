using DPTS.Applications.NoDistinctionOfRoles.categories.Dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.NoDistinctionOfRoles.categories.Queries
{
    public class GetCategoryIndexListQuery : IRequest<ServiceResult<IEnumerable<CategoryIndexListDto>>> 
    {
    }
}
