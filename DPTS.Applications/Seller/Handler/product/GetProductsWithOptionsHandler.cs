using DPTS.Applications.Seller.Dtos.product;
using DPTS.Applications.Seller.Query.product;
using DPTS.Applications.Shareds;
using MediatR;
using MediatR.Registration;

namespace DPTS.Applications.Seller.Handler.product
{
    public class GetProductsWithOptionsHandler : IRequestHandler<GetProductsWithOptionsQuery, ServiceResult<ProductListItemDto>>
    {
        public Task<ServiceResult<ProductListItemDto>> Handle(GetProductsWithOptionsQuery request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
