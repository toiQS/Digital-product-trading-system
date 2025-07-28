using DPTS.Applications.Admin.manage_product.dtos;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Admin.manage_product.Queries
{
    public class GetCategoiesQuery : IRequest<ServiceResult<CategoryDto>>
    {
    }
}
