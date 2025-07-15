using DPTS.Applications.Buyer.Queries.order;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles.order;

public class AddToCartHandler : IRequestHandler<AddToCartCommand, ServiceResult<string>>
{
    private readonly IOrderRepository _orderRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly ILogRepository _logRepository;
    private readonly IAdjustmentHandle _adjustmentHandle;
    private readonly ILogger<AddToCartHandler> _logger;

    public AddToCartHandler(
        IOrderRepository orderRepository,
        IProductRepository productRepository,
        IOrderItemRepository orderItemRepository,
        IUserProfileRepository userProfileRepository,
        ILogRepository logRepository,
        IAdjustmentHandle adjustmentHandle,
        ILogger<AddToCartHandler> logger)
    {
        _orderRepository = orderRepository;
        _productRepository = productRepository;
        _orderItemRepository = orderItemRepository;
        _userProfileRepository = userProfileRepository;
        _logRepository = logRepository;
        _adjustmentHandle = adjustmentHandle;
        _logger = logger;
    }

    public async Task<ServiceResult<string>> Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        // 1. Xác minh người dùng
        var profile = await _userProfileRepository.GetByUserIdAsync(request.UserId);
        if (profile == null)
        {
            _logger.LogError("User profile not found.");
            return ServiceResult<string>.Error("Không tìm thấy thông tin người dùng.");
        }

        // 2. Lấy danh sách đơn hàng chưa thanh toán
        var unpaidOrders = (await _orderRepository.GetByBuyerAsync(request.UserId))
                            .Where(o => !o.IsPaid)
                            .ToList();

        if (unpaidOrders.Count == 0)
        {
            // Không có đơn nào => tạo đơn mới
            return await AddItemWithNewOrder(request.UserId, request.ProjectId, request.Quantities);
        }

        if (unpaidOrders.Count > 1)
        {
            _logger.LogError("Multiple unpaid orders found for user {UserId}", request.UserId);
            return ServiceResult<string>.Error("Hệ thống phát hiện nhiều giỏ hàng chưa thanh toán. Vui lòng liên hệ hỗ trợ.");
        }

        var order = unpaidOrders.First();

        // 3. Thêm mới hoặc cập nhật sản phẩm trong giỏ hàng
        var itemResult = await UpdateOrAddItem(order.OrderId, request.ProjectId, request.Quantities);
        if (itemResult.Status == StatusResult.Errored)
            return itemResult;

        // 4. Cập nhật tổng tiền giỏ hàng
        try
        {
            var items = await _orderItemRepository.GetByOrderIdAsync(order.OrderId);
            order.TotalAmount = items.Sum(x => x.TotalPrice);
            order.UpdatedAt = DateTime.UtcNow;

            await _orderRepository.UpdateAsync(order);

            var log = new Log
            {
                LogId = Guid.NewGuid().ToString(),
                UserId = request.UserId,
                Action = "UpdateCartTotal",
                TargetId = order.OrderId,
                TargetType = "Order",
                CreatedAt = DateTime.UtcNow
            };

            await _logRepository.AddAsync(log);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update cart total for order {OrderId}", order.OrderId);
            return ServiceResult<string>.Error("Không thể cập nhật tổng tiền giỏ hàng.");
        }

        return ServiceResult<string>.Success("Đã thêm sản phẩm vào giỏ hàng.");
    }

    private async Task<ServiceResult<string>> UpdateOrAddItem(string orderId, string productId, int quantity)
    {
        // Lấy thông tin sản phẩm
        var product = await _productRepository.GetByIdAsync(productId);
        if (product == null)
        {
            _logger.LogError("Product not found: {ProductId}", productId);
            return ServiceResult<string>.Error("Không tìm thấy sản phẩm.");
        }

        // Tính toán giảm giá và giá cuối
        var priceResult = await _adjustmentHandle.HandleDiscountAndPriceForProduct(product);
        if (priceResult?.Data == null)
        {
            _logger.LogError("Price adjustment failed for product: {ProductId}", productId);
            return ServiceResult<string>.Error("Không thể tính giá sản phẩm.");
        }

        var items = await _orderItemRepository.GetByOrderIdAsync(orderId);
        var existingItem = items.FirstOrDefault(x => x.ProductId == productId);

        if (existingItem != null)
        {
            // Nếu sản phẩm đã có trong giỏ → cập nhật số lượng
            existingItem.Quantity += quantity;
            existingItem.PriceForeachProduct = priceResult.Data.FinalAmount;
            existingItem.TotalPrice = existingItem.Quantity * existingItem.PriceForeachProduct;

            try
            {
                await _orderItemRepository.UpdateAsync(existingItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update item in cart.");
                return ServiceResult<string>.Error("Không thể cập nhật sản phẩm trong giỏ hàng.");
            }
        }
        else
        {
            // Nếu sản phẩm chưa có → thêm mới
            var newItem = new OrderItem
            {
                OrderItemId = Guid.NewGuid().ToString(),
                OrderId = orderId,
                ProductId = productId,
                Quantity = quantity,
                PriceForeachProduct = priceResult.Data.FinalAmount,
                TotalPrice = priceResult.Data.FinalAmount * quantity
            };

            try
            {
                await _orderItemRepository.AddAsync(newItem);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to add new item to cart.");
                return ServiceResult<string>.Error("Không thể thêm sản phẩm vào giỏ hàng.");
            }
        }

        return ServiceResult<string>.Success("Thêm sản phẩm thành công.");
    }

    private async Task<ServiceResult<string>> AddItemWithNewOrder(string userId, string productId, int quantity)
    {
        // Tạo đơn hàng mới
        var newOrder = new Order
        {
            OrderId = Guid.NewGuid().ToString(),
            BuyerId = userId,
            IsPaid = false,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            TotalAmount = 0,
            OrderItems = []
        };

        try
        {
            await _orderRepository.AddAsync(newOrder);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to create new order for user {UserId}", userId);
            return ServiceResult<string>.Error("Không thể tạo đơn hàng mới.");
        }

        // Gọi lại hàm xử lý thêm sản phẩm vào đơn hàng mới
        return await UpdateOrAddItem(newOrder.OrderId, productId, quantity);
    }
}
