using DPTS.Applications.Buyer.Queries.review;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles.review
{
    public class LikeReviewHandler : IRequestHandler<LikeReviewCommand, ServiceResult<string>>
    {
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogRepository _logRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<LikeReviewHandler> _logger;

        public LikeReviewHandler(
            IProductReviewRepository productReviewRepository,
            IUserRepository userRepository,
            ILogRepository logRepository,
            IMessageRepository messageRepository,
            IProductRepository productRepository,
            ILogger<LikeReviewHandler> logger)
        {
            _productReviewRepository = productReviewRepository;
            _userRepository = userRepository;
            _logRepository = logRepository;
            _messageRepository = messageRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<string>> Handle(LikeReviewCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling LikeComment: UserId = {UserId}, ReviewId = {ReviewId}", request.UserId, request.ProjectReviewId);

            // Kiểm tra người dùng
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("User not found: {UserId}", request.UserId);
                return ServiceResult<string>.Error("Người dùng không tồn tại.");
            }

            // Kiểm tra bình luận sản phẩm
            var review = await _productReviewRepository.GetById(request.ProjectReviewId);
            if (review == null)
            {
                _logger.LogError("Product review not found: {ReviewId}", request.ProjectReviewId);
                return ServiceResult<string>.Error("Không tìm thấy đánh giá sản phẩm.");
            }

            // Lấy thông tin sản phẩm để gửi thông báo cho cửa hàng
            var product = await _productRepository.GetByIdAsync(review.ProductId);
            if (product == null)
            {
                _logger.LogError("Product not found: {ProductId}", review.ProductId);
                return ServiceResult<string>.Error("Không tìm thấy sản phẩm.");
            }

            // Cập nhật lượt thích (tăng lên 1 đơn vị)
            review.Likes += 1;

            // Ghi log hệ thống
            var log = new Log
            {
                LogId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                UserId = user.UserId,
                TargetId = review.ReviewId,
                TargetType = "ProductReview",
                Action = "LikeReview"
            };

            // Gửi tin nhắn hệ thống cho cửa hàng
            var message = new Message
            {
                MessageId = Guid.NewGuid().ToString(),
                IsSystem = true,
                ReceiverId = product.StoreId,
                ReceiverType = ParticipantType.Store,
                CreatedAt = DateTime.UtcNow,
                Content = $"[Hệ thống] Người dùng {user.Username} đã thích một đánh giá về sản phẩm \"{product.ProductName}\"."
            };

            try
            {
                await _productReviewRepository.UpdateAsync(review);
                await _logRepository.AddAsync(log);
                await _messageRepository.AddAsync(message);

                _logger.LogInformation("Successfully liked review: ReviewId = {ReviewId}", review.ReviewId);
                return ServiceResult<string>.Success("Bạn đã thích đánh giá này.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while liking comment: UserId = {UserId}, ReviewId = {ReviewId}", request.UserId, request.ProjectReviewId);
                return ServiceResult<string>.Error("Đã xảy ra lỗi khi thích đánh giá. Vui lòng thử lại sau.");
            }
        }
    }
}
