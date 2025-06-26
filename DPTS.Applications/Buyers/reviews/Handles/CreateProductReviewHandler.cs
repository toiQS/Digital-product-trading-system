using DPTS.Applications.Buyers.reviews.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyers.reviews.Handles
{
    public class CreateProductReviewHandler : IRequestHandler<CreateProductReviewCommand, ServiceResult<string>>
    {
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<CreateProductReviewHandler> _logger;
        private readonly ILogRepository _logRepository;
        private readonly IMessageRepository _messageRepository;

        public CreateProductReviewHandler(
            IProductReviewRepository productReviewRepository,
            IProductRepository productRepository,
            ILogger<CreateProductReviewHandler> logger,
            ILogRepository logRepository,
            IMessageRepository messageRepository)
        {
            _productReviewRepository = productReviewRepository;
            _productRepository = productRepository;
            _logger = logger;
            _logRepository = logRepository;
            _messageRepository = messageRepository;
        }

        public async Task<ServiceResult<string>> Handle(CreateProductReviewCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Bắt đầu xử lý đánh giá sản phẩm: ProductId={ProductId}, BuyerId={BuyerId}", request.ProductId, request.BuyerId);
            try
            {
                var product = await _productRepository.GetByIdAsync(request.ProductId);
                if (product == null)
                {
                    _logger.LogWarning("Không tìm thấy sản phẩm với ProductId: {ProductId}", request.ProductId);
                    return ServiceResult<string>.Error("Sản phẩm không tồn tại.");
                }

                var review = new ProductReview
                {
                    ReviewId = Guid.NewGuid().ToString(),
                    ProductId = request.ProductId,
                    Comment = request.ReviewContent,
                    ReviewTitle = request.ReviewTitle,
                    CreatedAt = DateTime.UtcNow,
                    Likes = 0,
                    RatingOverall = request.OverallRating,
                    RatingQuality = request.QualityRating,
                    RatingUsability = request.UsabilityRating,
                    RatingValue = request.ValueRating,
                    RecommendToOthers = request.RecommendToOthers,
                    UserId = request.BuyerId
                };

                var log = new Log
                {
                    LogId = Guid.NewGuid().ToString(),
                    UserId = request.BuyerId,
                    Action = $"Người dùng {request.BuyerId} đã đánh giá sản phẩm {request.ProductId}",
                    CreatedAt = DateTime.UtcNow
                };

                var notification = new Message
                {
                    IsSystem = true,
                    Content = $"Bạn vừa nhận được một đánh giá mới cho sản phẩm {product.ProductName}.",
                    ReceiverStoreId = product.StoreId,
                    CreatedAt = DateTime.UtcNow
                };

                try
                {
                    await _productReviewRepository.AddAsync(review);
                    await _logRepository.AddAsync(log);
                    await _messageRepository.AddAsync(notification);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi lưu đánh giá hoặc thông báo cho ProductId: {ProductId}", request.ProductId);
                    return ServiceResult<string>.Error("Không thể lưu đánh giá. Vui lòng thử lại.");
                }

                _logger.LogInformation("Đánh giá sản phẩm thành công: ProductId={ProductId}, BuyerId={BuyerId}", request.ProductId, request.BuyerId);
                return ServiceResult<string>.Success("Đánh giá thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định khi xử lý đánh giá sản phẩm.");
                return ServiceResult<string>.Error("Đã xảy ra lỗi trong quá trình xử lý đánh giá.");
            }
        }
    }
}
