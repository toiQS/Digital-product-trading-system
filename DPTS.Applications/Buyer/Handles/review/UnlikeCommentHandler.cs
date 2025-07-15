using DPTS.Applications.Buyer.Queries.review;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles.review
{
    public class UnlikeCommentHandler : IRequestHandler<UnlikeCommentCommand, ServiceResult<string>>
    {
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly IUserRepository _userRepository;
        private readonly ILogRepository _logRepository;
        private readonly IMessageRepository _messageRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<UnlikeCommentHandler> _logger;

        public UnlikeCommentHandler(
            IProductReviewRepository productReviewRepository,
            IUserRepository userRepository,
            ILogRepository logRepository,
            IMessageRepository messageRepository,
            IProductRepository productRepository,
            ILogger<UnlikeCommentHandler> logger)
        {
            _productReviewRepository = productReviewRepository;
            _userRepository = userRepository;
            _logRepository = logRepository;
            _messageRepository = messageRepository;
            _productRepository = productRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<string>> Handle(UnlikeCommentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling UnlikeComment: UserId = {UserId}, ReviewId = {ReviewId}", request.UserId, request.ProjectReviewId);

            // Kiểm tra người dùng
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("User not found: {UserId}", request.UserId);
                return ServiceResult<string>.Error("Người dùng không tồn tại.");
            }

            // Kiểm tra review
            var review = await _productReviewRepository.GetById(request.ProjectReviewId);
            if (review == null)
            {
                _logger.LogError("Review not found: {ReviewId}", request.ProjectReviewId);
                return ServiceResult<string>.Error("Không tìm thấy đánh giá.");
            }

            // Kiểm tra sản phẩm để gửi thông báo
            var product = await _productRepository.GetByIdAsync(review.ProductId);
            if (product == null)
            {
                _logger.LogError("Product not found: {ProductId}", review.ProductId);
                return ServiceResult<string>.Error("Không tìm thấy sản phẩm.");
            }

            // Trừ like nhưng không để nhỏ hơn 0
            review.Likes = Math.Max(0, review.Likes - 1);

            // Ghi log
            var log = new Log
            {
                LogId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                UserId = user.UserId,
                TargetId = review.ReviewId,
                TargetType = "ProductReview",
                Action = "UnlikeReview"
            };

            // Gửi tin nhắn cho cửa hàng
            var message = new Message
            {
                MessageId = Guid.NewGuid().ToString(),
                IsSystem = true,
                ReceiverId = product.StoreId,
                ReceiverType = ParticipantType.Store,
                CreatedAt = DateTime.UtcNow,
                Content = $"[Hệ thống] Người dùng {user.Username} đã bỏ thích đánh giá về sản phẩm \"{product.ProductName}\"."
            };

            try
            {
                await _productReviewRepository.UpdateAsync(review);
                await _logRepository.AddAsync(log);
                await _messageRepository.AddAsync(message);

                _logger.LogInformation("Unlike successful: ReviewId = {ReviewId}", review.ReviewId);
                return ServiceResult<string>.Success("Bạn đã bỏ thích đánh giá này.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during unlike operation: UserId = {UserId}, ReviewId = {ReviewId}", request.UserId, request.ProjectReviewId);
                return ServiceResult<string>.Error("Đã xảy ra lỗi khi bỏ thích đánh giá.");
            }
        }
    }
}
