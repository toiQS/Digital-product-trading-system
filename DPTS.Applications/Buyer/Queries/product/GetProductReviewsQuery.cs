using DPTS.Applications.Buyer.Dtos.product;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.product
{
    public class GetProductReviewsQuery : IRequest<ServiceResult<IEnumerable<ProductReviewIndexDto>>>
    {
        public string ProductId { get; set; } = string.Empty;
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
