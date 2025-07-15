using DPTS.Applications.Buyer.Queries.complaint;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles.complaint
{
    public class AddComplaintHandler : IRequestHandler<AddComplaintCommand, ServiceResult<string>>
    {
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IComplaintRepository _complaintRepository;
        private readonly ILogger<AddComplaintCommand> _logger;
        private readonly IProductRepository _productRepository;
        private readonly IEscrowRepository _escrowRepository;
        private readonly ILogRepository _logRepository;
        private readonly IMessageRepository _messageRepository;

        public AddComplaintHandler(
            IOrderItemRepository orderItemRepository,
            IUserProfileRepository userProfileRepository,
            IComplaintRepository complaintRepository,
            ILogger<AddComplaintCommand> logger,
            IProductRepository productRepository,
            IEscrowRepository escrowRepository,
            ILogRepository logRepository,
            IMessageRepository messageRepository)
        {
            _orderItemRepository = orderItemRepository;
            _userProfileRepository = userProfileRepository;
            _complaintRepository = complaintRepository;
            _logger = logger;
            _productRepository = productRepository;
            _escrowRepository = escrowRepository;
            _logRepository = logRepository;
            _messageRepository = messageRepository;
        }

        public async Task<ServiceResult<string>> Handle(AddComplaintCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling AddComplaint for UserId = {UserId}, ItemId = {ItemId}", request.UserId, request.ItemId);

            // Lấy thông tin người dùng
            var user = await _userProfileRepository.GetByUserIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("User not found: {UserId}", request.UserId);
                return ServiceResult<string>.Error("Người dùng không tồn tại.");
            }

            // Lấy thông tin sản phẩm trong đơn hàng
            var item = await _orderItemRepository.GetByIdAsync(request.ItemId);
            if (item == null)
            {
                _logger.LogError("Order item not found: {ItemId}", request.ItemId);
                return ServiceResult<string>.Error("Không tìm thấy sản phẩm trong đơn hàng.");
            }

            // Lấy thông tin sản phẩm
            var product = await _productRepository.GetByIdAsync(item.ProductId);
            if (product == null)
            {
                _logger.LogError("Product not found: {ProductId}", item.ProductId);
                return ServiceResult<string>.Error("Không tìm thấy thông tin sản phẩm.");
            }

            // Lấy danh sách escrow của đơn hàng
            var escrows = await _escrowRepository.GetByOrderIdAsync(item.OrderId);
            var escrow = escrows.FirstOrDefault(e => e.StoreId == product.StoreId);
            if (escrow == null)
            {
                _logger.LogError("Escrow not found for OrderId = {OrderId}, StoreId = {StoreId}", item.OrderId, product.StoreId);
                return ServiceResult<string>.Error("Không tìm thấy giao dịch liên quan.");
            }

            // Kiểm tra xem khiếu nại có đang nằm trong thời gian xử lý hay không
            var now = DateTime.Now;
            if (escrow.Expired < now || escrow.CreatedAt > now)
            {
                _logger.LogWarning("Complaint rejected: Not in processing window. Now = {Now}, CreatedAt = {CreatedAt}, Expired = {Expired}", now, escrow.CreatedAt, escrow.Expired);
                return ServiceResult<string>.Error("Bạn chỉ có thể khiếu nại trong thời gian xử lý đơn hàng.");
            }

            // Tạo khiếu nại mới
            var complaint = new Complaint
            {
                ComplaintId = Guid.NewGuid().ToString(),
                CreatedAt = now,
                UpdatedAt = now,
                UserId = request.UserId,
                EscrowId = escrow.EscrowId,
                ProductId = product.ProductId,
                Description = request.Description,
                Title = request.ComplaintType.ToString(),
                Status = ComplaintStatus.Pending
            };

            // Tạo tin nhắn hệ thống
            var message = new Message
            {
                MessageId = Guid.NewGuid().ToString(),
                CreatedAt = now,
                IsSystem = true,
                ReceiverId = product.StoreId,
                ReceiverType = ParticipantType.Store,
                Content = "[Hệ thống] Người mua đã tạo khiếu nại về đơn hàng."
            };

            // Ghi nhật ký hệ thống
            var log = new Log
            {
                LogId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                UserId = user.UserId,
                TargetId = complaint.ComplaintId,
                TargetType = "Complaint",
                Action = "BuyerCreatedComplaint"
            };
            foreach (var image in request.ComplaintImages)
            {
                var complaintImage = new ComplaintImage()
                {
                    ImageId = Guid.NewGuid().ToString(),
                    ComplaintId = complaint.ComplaintId,
                    CreateAt = DateTime.UtcNow,
                    ImagePath = image,
                };
                complaint.Images.Add(complaintImage);
            }
            try
            {
                // Cập nhật trạng thái giao dịch
                escrow.Status = EscrowStatus.Complaint;
                escrow.UpdatedAt = DateTime.UtcNow;
                await _escrowRepository.UpdateAsync(escrow);

                // Lưu dữ liệu vào các bảng liên quan
                await _complaintRepository.AddAsync(complaint);
                
                await _messageRepository.AddAsync(message);
                await _logRepository.AddAsync(log);

                _logger.LogInformation("Complaint successfully created: ComplaintId = {ComplaintId}", complaint.ComplaintId);
                return ServiceResult<string>.Success("Tạo khiếu nại thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while creating complaint for UserId = {UserId}", request.UserId);
                return ServiceResult<string>.Error("Có lỗi xảy ra trong quá trình tạo khiếu nại. Vui lòng thử lại sau.");
            }
        }
    }
}
