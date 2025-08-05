using DPTS.Applications.Buyer.Dtos.product;
using DPTS.Applications.Buyer.Queries.product;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Server.HttpSys;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles.product
{
    public class GetProductReviewsHandler : IRequestHandler<GetProductReviewsQuery, ServiceResult<IEnumerable<ProductReviewIndexDto>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly ILogger<GetProductReviewsHandler> _logger;

        public GetProductReviewsHandler(IUserRepository userRepository, IProductReviewRepository productReviewRepository, IUserProfileRepository userProfileRepository, ILogger<GetProductReviewsHandler> logger)
        {
            _userRepository = userRepository;
            _productReviewRepository = productReviewRepository;
            _userProfileRepository = userProfileRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<ProductReviewIndexDto>>> Handle(GetProductReviewsQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling get product reviews for ProductId: {ProductId}", request.ProductId);
            var productReviews = await _productReviewRepository.GetByProductIdAsync(request.ProductId);
            var result = new List<ProductReviewIndexDto>();
            foreach (var item in productReviews)
            {
                var user = await _userRepository.GetByIdAsync(item.UserId);
                if (user == null)
                {
                    _logger.LogError("Không tìm thấy người dùng với ID: {UserId}", item.UserId);
                    return ServiceResult<IEnumerable<ProductReviewIndexDto>>.Error("Không tìm thấy người dùng đánh giá.");
                }

                var profile = await _userProfileRepository.GetByUserIdAsync(item.UserId);
                if (profile == null)
                {
                    _logger.LogError("Không tìm thấy hồ sơ người dùng với ID: {UserId}", item.UserId);
                    return ServiceResult<IEnumerable<ProductReviewIndexDto>>.Error("Không tìm thấy hồ sơ người dùng.");
                }

                result.Add(new ProductReviewIndexDto()
                {
                    FullName = profile.FullName ?? "Error",
                    Comment = item.Comment,
                    UserName = user.Username,
                    CreatedAt = item.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    ImageUser = profile.ImageUrl ?? "Error",
                    Likes = item.Likes,
                    RatingOverall = item.RatingValue,
                    ReviewId = item.ReviewId,
                    UserId = user.UserId,
                });
            }
            if(request.PageSize > 0 && request.PageIndex >= 0)
            {
                result = result.Skip(request.PageIndex * request.PageSize).Take(request.PageSize).ToList();
            }
            return ServiceResult<IEnumerable<ProductReviewIndexDto>>.Success(result);
        }
    }
}
