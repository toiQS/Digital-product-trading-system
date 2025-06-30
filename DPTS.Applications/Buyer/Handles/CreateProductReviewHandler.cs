using DPTS.Applications.Buyer.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles;

public class CreateProductReviewHandler : IRequestHandler<CreateProductReviewQuery, ServiceResult<string>>
{
    private readonly IProductRepository _productRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ILogRepository _logRepository;
    private readonly IProductReviewRepository _productReviewRepository;
    private readonly IMessageRepository _messageRepository;
    private readonly ILogger<CreateProductReviewHandler> _logger;

    public CreateProductReviewHandler(
        IProductRepository productRepository,
        IUserProfileRepository userProfileRepository,
        ILogRepository logRepository,
        IProductReviewRepository productReviewRepository,
        IMessageRepository messageRepository,
        ILogger<CreateProductReviewHandler> logger)
    {
        _productRepository = productRepository;
        _userProfileRepository = userProfileRepository;
        _logRepository = logRepository;
        _productReviewRepository = productReviewRepository;
        _messageRepository = messageRepository;
        _logger = logger;
    }

    public async Task<ServiceResult<string>> Handle(CreateProductReviewQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("User {UserId} is submitting a review for product {ProductId}", request.BuyerId, request.ProductId);

        // Lấy thông tin sản phẩm
        var product = await _productRepository.GetByIdAsync(request.ProductId);
        if (product == null)
        {
            _logger.LogError("Product not found: {ProductId}", request.ProductId);
            return ServiceResult<string>.Error("Không tìm thấy sản phẩm.");
        }

        // Lấy thông tin người mua
        var profile = await _userProfileRepository.GetByUserIdAsync(request.BuyerId);
        if (profile == null)
        {
            _logger.LogError("User profile not found: {UserId}", request.BuyerId);
            return ServiceResult<string>.Error("Không tìm thấy thông tin người dùng.");
        }

        // Tạo đánh giá
        var review = new ProductReview
        {
            ReviewId = Guid.NewGuid().ToString(),
            ProductId = product.ProductId,
            UserId = profile.UserId,
            ReviewTitle = request.ReviewTitle,
            Comment = request.ReviewContent,
            CreatedAt = DateTime.UtcNow,
            Likes = 0,
            RatingValue = request.ValueRating,
            RatingQuality = request.QualityRating,
            RatingUsability = request.UsabilityRating,
            RecommendToOthers = request.RecommendToOthers,
            RatingOverall = CalculateAverageRating(request)
        };

        // Tạo bản ghi log
        var log = new Log
        {
            LogId = Guid.NewGuid().ToString(),
            UserId = profile.UserId,
            Action = "CreateProductReview",
            TargetId = product.ProductId,
            TargetType = "Product",
            CreatedAt = DateTime.UtcNow,
            UserType = "User",
            IpAddress = "",     // Nên truyền từ request nếu cần
            UserAgent = ""      // Nên truyền từ request nếu cần
        };

        // Tạo message hệ thống gửi tới store
        var message = new Message
        {
            MessageId = Guid.NewGuid().ToString(),
            Content = $"Sản phẩm \"{product.ProductName}\" vừa nhận được đánh giá mới.",
            CreatedAt = DateTime.UtcNow,
            IsSystem = true,
            ReceiverId = product.StoreId,
            ReceiverType = ParticipantType.Store
        };

        try
        {
            await _productReviewRepository.AddAsync(review);
            await _logRepository.AddAsync(log);
            await _messageRepository.AddAsync(message);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create product review for product {ProductId} by user {UserId}", request.ProductId, request.BuyerId);
            return ServiceResult<string>.Error("Đánh giá sản phẩm thất bại. Vui lòng thử lại.");
        }

        _logger.LogInformation("Product review submitted successfully by user {UserId} for product {ProductId}", request.BuyerId, request.ProductId);
        return ServiceResult<string>.Success("Đánh giá sản phẩm thành công.");
    }

    /// <summary>
    /// Tính điểm trung bình của đánh giá.
    /// </summary>
    private double CalculateAverageRating(CreateProductReviewQuery request)
    {
        return (double)(request.ValueRating + request.QualityRating + request.UsabilityRating) / 3;
    }
}
