using DPTS.Applications.case_buyer.homepage.dtos;
using DPTS.Applications.shareds;
using MediatR;

namespace DPTS.Applications.case_buyer.homepage.models
{
    public class GetCategoriesOutStandingIndexQuery : IRequest<ServiceResult<IEnumerable<CategoriesOutstandingIndexDto>>>
    {
    }
}
