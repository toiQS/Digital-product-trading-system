using DPTS.Applications.Buyer.Dtos.product;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.product
{
    public class GetRatingQuery : IRequest<ServiceResult<IEnumerable<RateIndexDto>>>
    {
    }
}
