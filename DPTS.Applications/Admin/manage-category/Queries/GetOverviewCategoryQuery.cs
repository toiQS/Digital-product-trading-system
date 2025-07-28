using DPTS.Applications.Admin.manage_category.dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Admin.manage_category.Queries
{
    public class GetOverviewCategoryQuery : IRequest<ServiceResult<OverviewCategoryDto>>
    {
        public string UserId { get; set; }
    }
}
