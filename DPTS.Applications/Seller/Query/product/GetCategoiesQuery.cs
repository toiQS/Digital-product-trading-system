using DPTS.Applications.Seller.Dtos.product;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Seller.Query.product
{
    public class GetCategoiesQuery : IRequest<ServiceResult<CategoryDto>>
    {
    }
}
