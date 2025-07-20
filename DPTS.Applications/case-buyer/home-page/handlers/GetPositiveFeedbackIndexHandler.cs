using DPTS.Applications.case_buyer.homepage.dtos;
using DPTS.Applications.case_buyer.homepage.models;
using DPTS.Applications.shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repositories.Contracts.ProductReviews;
using DPTS.Infrastructures.Repositories.Contracts.UserProfiles;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.case_buyer.homepage.handlers
{
    public class GetPositiveFeedbackIndexHandler : IRequestHandler<GetPositiveFeedbackIndexQuery, ServiceResult<IEnumerable<PositiveFeedbackIndexDto>>>
    {
        private readonly IProductReviewQuery _productReviewQuery;
        private readonly IUserProfileQuery _userProfileQuery;
        private readonly ILogger<GetPositiveFeedbackIndexHandler> _logger;

        public GetPositiveFeedbackIndexHandler(IProductReviewQuery productReviewQuery, IUserProfileQuery userProfileQuery, ILogger<GetPositiveFeedbackIndexHandler> logger)
        {
            _productReviewQuery = productReviewQuery;
            _userProfileQuery = userProfileQuery;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<PositiveFeedbackIndexDto>>> Handle(GetPositiveFeedbackIndexQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetPositiveFeedbackIndexQuery");
            IEnumerable<ProductReview> positiveFeedbacks = await _productReviewQuery.GetPositiveFeedbacksAsync(take:10, cancellationToken);
            if (positiveFeedbacks == null || !positiveFeedbacks.Any())
            {
                _logger.LogWarning("No positive feedbacks found");
                return ServiceResult<IEnumerable<PositiveFeedbackIndexDto>>.Error("Không tìm thấy phản hồi nào");
            }
            var result = new List<PositiveFeedbackIndexDto>(); 
            foreach (var feedback in positiveFeedbacks)
            {
                UserProfile? userProfile = await _userProfileQuery.GetByIdAsync(feedback.UserId, cancellationToken);
                if( userProfile == null)
                {
                    _logger.LogWarning($"User profile not found for UserId: {feedback.UserId}");
                    return ServiceResult<IEnumerable<PositiveFeedbackIndexDto>>.Error("Không tìm thấy thông tin người dùng");
                }
                var positiveFeedback = new PositiveFeedbackIndexDto
                {
                    FullName = userProfile.FullName ?? "Unknown User",
                    Bio = userProfile.Bio ?? "No bio available",
                    Content = feedback.Comment ?? "No content available",
                };
                result.Add(positiveFeedback);
            }
            return ServiceResult<IEnumerable<PositiveFeedbackIndexDto>>.Success(result);
        }
    }
}
