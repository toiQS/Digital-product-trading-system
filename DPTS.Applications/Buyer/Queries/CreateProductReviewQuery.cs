using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Buyer.Queries
{
    public class CreateProductReviewQuery : IRequest<ServiceResult<string>>
    {
        public string ProductId { get; set; } = string.Empty;
        public string BuyerId { get; set; } = string.Empty;

        public int QualityRating { get; set; }            // Chất lượng
        public int ValueRating { get; set; }              // Giá trị
        public int UsabilityRating { get; set; }          // Dễ sử dụng

        public string ReviewTitle { get; set; } = string.Empty;
        public string ReviewContent { get; set; } = string.Empty;
        public bool RecommendToOthers { get; set; }
    }
}
