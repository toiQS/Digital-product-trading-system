using DPTS.Applications.Buyer.Dtos;
using DPTS.Applications.Buyer.Dtos.order;
using DPTS.Applications.Buyer.Handles.chat;
using DPTS.Applications.Buyer.Queries.order;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles.order;

public class GetCheckoutHandler : IRequestHandler<GetCheckoutQuery, ServiceResult<CheckoutDto>>
{
    private readonly IAdjustmentHandle _adjustmentHandle;
    private readonly IProductImageRepository _productImageRepository;
    private readonly IProductRepository _productRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly ILogger<GetChatWithStoreHandler> _logger;

    public GetCheckoutHandler(
        IAdjustmentHandle adjustmentHandle,
        IProductImageRepository productImageRepository,
        IProductRepository productRepository,
        IOrderRepository orderRepository,
        IOrderItemRepository orderItemRepository,
        ILogRepository logRepository,
        IUserProfileRepository userProfileRepository,
        IWalletRepository walletRepository,
        IPaymentMethodRepository paymentMethodRepository,
        ILogger<GetChatWithStoreHandler> logger)
    {
        _adjustmentHandle = adjustmentHandle;
        _productImageRepository = productImageRepository;
        _productRepository = productRepository;
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _logRepository = logRepository;
        _userProfileRepository = userProfileRepository;
        _walletRepository = walletRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _logger = logger;
    }

    public async Task<ServiceResult<CheckoutDto>> Handle(GetCheckoutQuery request, CancellationToken cancellationToken)
    {
        CheckoutDto result = new();

        // B1: Xác thực hồ sơ người dùng
        var profile = await _userProfileRepository.GetByUserIdAsync(request.BuyerId);
        if (profile == null)
        {
            _logger.LogError("User profile not found: {UserId}", request.BuyerId);
            return ServiceResult<CheckoutDto>.Error("Không tìm thấy hồ sơ người dùng.");
        }

        // B2: Lấy đơn hàng chưa thanh toán
        var unpaidOrders = (await _orderRepository.GetByBuyerAsync(request.BuyerId))
            .Where(x => !x.IsPaid).ToList();

        if (unpaidOrders.Count == 0)
            return ServiceResult<CheckoutDto>.Error("Không có giỏ hàng nào.");

        if (unpaidOrders.Count > 1)
        {
            _logger.LogError("Multiple unpaid orders found for user: {UserId}", request.BuyerId);
            return ServiceResult<CheckoutDto>.Error("Có nhiều giỏ hàng chưa thanh toán. Vui lòng liên hệ hỗ trợ.");
        }

        var order = unpaidOrders.First();

        // B3: Duyệt từng item trong đơn hàng
        var orderItems = await _orderItemRepository.GetByOrderIdAsync(order.OrderId);
        foreach (var item in orderItems)
        {
            var product = await _productRepository.GetByIdAsync(item.ProductId, includeStore: true);
            if (product == null)
            {
                _logger.LogError("Product not found: {ProductId}", item.ProductId);
                return ServiceResult<CheckoutDto>.Error("Không tìm thấy sản phẩm trong giỏ hàng.");
            }

            var priceResult = await _adjustmentHandle.HandleDiscountAndPriceForProduct(product);
            if (priceResult.Status == StatusResult.Errored)
            {
                _logger.LogError("Failed to calculate price for product: {ProductId}", product.ProductId);
                return ServiceResult<CheckoutDto>.Error("Không thể tính giá sản phẩm.");
            }

            try
            {
                item.PriceForeachProduct = priceResult.Data.FinalAmount;
                item.TotalPrice = item.PriceForeachProduct * item.Quantity;
                await _orderItemRepository.UpdateAsync(item);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update order item: {ItemId}", item.OrderItemId);
                return ServiceResult<CheckoutDto>.Error("Không thể cập nhật sản phẩm trong đơn hàng.");
            }

            var image = await _productImageRepository.GetPrimaryAsync(product.ProductId);
            if (image == null)
            {
                _logger.LogError("Primary image not found for product: {ProductId}", product.ProductId);
                return ServiceResult<CheckoutDto>.Error("Không tìm thấy ảnh sản phẩm.");
            }

            result.Items.Add(new CheckoutItemDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                StoreName = product.Store.StoreName,
                ProductImage = image.ImagePath,
                Price = item.PriceForeachProduct,
                Amount = item.TotalPrice
            });
        }

        // B4: Cập nhật tổng tiền đơn hàng
        try
        {
            order.TotalAmount = orderItems.Sum(x => x.TotalPrice);
            order.UpdatedAt = DateTime.UtcNow;
            await _orderRepository.UpdateAsync(order);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update order total.");
            return ServiceResult<CheckoutDto>.Error("Không thể cập nhật tổng tiền đơn hàng.");
        }

        // B5: Tính toán giảm giá/khuyến mãi cho toàn bộ đơn hàng
        var discountResult = await _adjustmentHandle.HandleDiscountForOrderAndPayment(order: order);
        if (discountResult.Status == StatusResult.Errored)
        {
            _logger.LogError("Failed to apply discount for order: {OrderId}", order.OrderId);
            return ServiceResult<CheckoutDto>.Error("Không thể áp dụng khuyến mãi cho đơn hàng.");
        }

        result.Summary = new CheckoutSummaryDto
        {
            DiscountId = discountResult.Data.AdjustmentRuleId,
            DiscountValue = discountResult.Data.DiscountValue,
            FinalAmount = discountResult.Data.FinalAmount
        };

        // B6: Gợi ý phương thức thanh toán
        var wallet = await _walletRepository.GetByUserIdAsync(request.BuyerId);
        if (wallet != null)
        {
            result.PaymentMethods.Add(new PaymentMethodOptionDto
            {
                PaymentMethodId = wallet.WalletId,
                IsRecommended = true,
                AvailableBalance = wallet.Balance,
                MethodName = "Ví sàn",
                MethodCode = "wallet",
                Description = ""
            });
        }

        var externalMethods = await _paymentMethodRepository.GetByUserIdAsync(request.BuyerId);
        foreach (var method in externalMethods)
        {
            result.PaymentMethods.Add(new PaymentMethodOptionDto
            {
                PaymentMethodId = method.PaymentMethodId,
                IsRecommended = true,
                MethodName = "Phương thức khác",
                MethodCode = method.Provider.ToString()
            });
        }

        // B7: Ghi log hành động
        var log = new Log
        {
            LogId = Guid.NewGuid().ToString(),
            UserId = request.BuyerId,
            Action = "ViewCheckout",
            CreatedAt = DateTime.UtcNow,
            TargetId = order.OrderId,
            TargetType = "Order"
        };

        try
        {
            await _logRepository.AddAsync(log);
            return ServiceResult<CheckoutDto>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to log checkout action.");
            return ServiceResult<CheckoutDto>.Error("Không thể ghi lịch sử thao tác.");
        }
    }
}

