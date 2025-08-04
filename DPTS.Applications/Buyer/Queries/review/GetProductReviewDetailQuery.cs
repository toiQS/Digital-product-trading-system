using DPTS.Applications.Buyer.Dtos.review;
using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries.review
{
    public class GetProductReviewDetailQuery : IRequest<ServiceResult<ProductReviewDetailDto>>
    {
        public string ProductReviewId { get; set; } 
    }
}
