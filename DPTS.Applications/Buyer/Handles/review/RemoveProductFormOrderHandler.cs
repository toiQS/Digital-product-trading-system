using DPTS.Applications.Buyer.Queries.order;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles.review
{
    public class RemoveProductFormOrderHandler : IRequestHandler<RemoveProductFormOrderCommand, ServiceResult<string>>
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IProductRepository _productRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ILogRepository _logRepository;
        private readonly ILogger<RemoveProductFormOrderHandler> _logger;

        public RemoveProductFormOrderHandler(
            IUserProfileRepository userProfileRepository,
            IProductRepository productRepository,
            IOrderRepository orderRepository,
            IOrderItemRepository orderItemRepository,
            ILogRepository logRepository,
            ILogger<RemoveProductFormOrderHandler> logger)
        {
            _userProfileRepository = userProfileRepository;
            _productRepository = productRepository;
            _orderRepository = orderRepository;
            _orderItemRepository = orderItemRepository;
            _logRepository = logRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<string>> Handle(RemoveProductFormOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Start handling RemoveProductFormOrderCommand for UserId = {UserId}, ProductId = {ProductId}", request.UserId, request.ProductId);

            // 1. Kiểm tra người dùng
            var user = await _userProfileRepository.GetByUserIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("User not found: {UserId}", request.UserId);
                return ServiceResult<string>.Error("Không tìm thấy người dùng.");
            }

            // 2. Lấy đơn hàng chưa thanh toán của người dùng
            var unpaidOrders = (await _orderRepository.GetByBuyerAsync(request.UserId))
                               .Where(x => !x.IsPaid)
                               .ToList();

            if (unpaidOrders.Count != 1)
            {
                _logger.LogError("Invalid number of unpaid orders: {Count}", unpaidOrders.Count);
                return ServiceResult<string>.Error("Không thể xác định đơn hàng chưa thanh toán.");
            }

            var order = unpaidOrders.First();

            // 3. Tìm sản phẩm trong giỏ hàng (đơn hàng chi tiết)
            var orderItem = (await _orderItemRepository.GetAllAsync())
                            .FirstOrDefault(x => x.ProductId == request.ProductId && x.OrderId == order.OrderId);

            if (orderItem == null)
            {
                _logger.LogError("Product not found in the order: ProductId = {ProductId}", request.ProductId);
                return ServiceResult<string>.Error("Không tìm thấy sản phẩm trong đơn hàng.");
            }

            // 4. Kiểm tra sản phẩm tồn tại
            var product = await _productRepository.GetByIdAsync(request.ProductId);
            if (product == null)
            {
                _logger.LogError("Product not found: {ProductId}", request.ProductId);
                return ServiceResult<string>.Error("Không tìm thấy thông tin sản phẩm.");
            }

            // 5. Kiểm tra số lượng yêu cầu hợp lệ
            if (orderItem.Quantity < request.Quantity)
            {
                _logger.LogError("Invalid quantity to remove. Requested: {Requested}, Available: {Available}", request.Quantity, orderItem.Quantity);
                return ServiceResult<string>.Error("Số lượng cần xóa vượt quá số lượng trong đơn hàng.");
            }

            // 6. Cập nhật hoặc xóa sản phẩm khỏi đơn hàng
            orderItem.Quantity -= request.Quantity;

            // 7. Ghi log thao tác
            var log = new Log
            {
                LogId = Guid.NewGuid().ToString(),
                UserId = user.UserId,
                Action = "RemoveProductFromOrder",
                TargetId = orderItem.OrderItemId,
                TargetType = "OrderItem",
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                if (orderItem.Quantity > 0)
                {
                    await _orderItemRepository.UpdateAsync(orderItem);
                }
                else
                {
                    await _orderItemRepository.DeleteAsync(orderItem.OrderItemId);
                }

                await _logRepository.AddAsync(log);

                _logger.LogInformation("Product removed from order successfully: OrderId = {OrderId}, ProductId = {ProductId}", order.OrderId, product.ProductId);
                return ServiceResult<string>.Success("Xóa sản phẩm khỏi đơn hàng thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to remove product from order: OrderId = {OrderId}, ProductId = {ProductId}", order.OrderId, product.ProductId);
                return ServiceResult<string>.Error("Đã xảy ra lỗi khi xóa sản phẩm khỏi đơn hàng.");
            }
        }
    }
}
