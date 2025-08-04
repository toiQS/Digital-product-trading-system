using DPTS.Applications.Buyer.Dtos.review;
using DPTS.Applications.Buyer.Queries.review;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles.review
{
    public class GetProductReviewDetailHandler : IRequestHandler<GetProductReviewDetailQuery, ServiceResult<ProductReviewDetailDto>>
    {

        private readonly IProductReviewRepository _productReviewRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly ILogger<GetProductReviewDetailHandler> _logger;

        public GetProductReviewDetailHandler(IProductReviewRepository productReviewRepository, IUserProfileRepository userProfileRepository, ILogger<GetProductReviewDetailHandler> logger)
        {
            _productReviewRepository = productReviewRepository;
            _userProfileRepository = userProfileRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<ProductReviewDetailDto>> Handle(GetProductReviewDetailQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetProductReviewDetailQuery for ProductReviewId: {ProductReviewId}", request.ProductReviewId);
            var review = await _productReviewRepository.GetById(request.ProductReviewId);
            if (review == null)
            {
                _logger.LogWarning("Product review not found for ID: {ProductReviewId}", request.ProductReviewId);
                return ServiceResult<ProductReviewDetailDto>.Error("Không tìm thấy cửa hàng");
            }
            var profile = await _userProfileRepository.GetByUserIdAsync(review.UserId);
            if (profile == null)
            {
                _logger.LogWarning("User profile not found for UserId: {UserId}", review.UserId);
                return ServiceResult<ProductReviewDetailDto>.Error("Không tìm thấy người dùng");
            }
            var result = new ProductReviewDetailDto
            {
                ReviewId = review.ReviewId,
                    UserId = review.UserId,
                    FullName = profile.FullName ?? "Error",
                AvatarUrl = profile.ImageUrl ?? "Error",
                Title = review.ReviewTitle,
                Comment = review.Comment,
                RatingOverall = review.RatingOverall,
                RatingQuality = review.RatingQuality,
                RatingValue = review.RatingValue,
                RatingUsability = review.RatingUsability,
                RecommendToOthers = review.RecommendToOthers,
            };
            return ServiceResult<ProductReviewDetailDto>.Success(result);
        }
    }
}
