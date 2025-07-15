using DPTS.Applications.Buyer.Dtos.order;
using DPTS.Applications.Buyer.Queries.order;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

public class CheckBuyNowHandler : IRequestHandler<CheckBuyNowQuery, ServiceResult<CheckoutDto>>
{
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IAdjustmentHandle _adjustmentHandle;
    private readonly IProductImageRepository _productImageRepository;
    private readonly IProductRepository _productRepository;
    private readonly IPaymentMethodRepository _paymentMethodRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly IWalletRepository _walletRepository;
    private readonly ILogger<CheckBuyNowHandler> _logger;

    public CheckBuyNowHandler(
        IUserProfileRepository userProfileRepository,
        IAdjustmentHandle adjustmentHandle,
        IProductImageRepository productImageRepository,
        IProductRepository productRepository,
        IPaymentMethodRepository paymentMethodRepository,
        IStoreRepository storeRepository,
        IWalletRepository walletRepository,
        ILogger<CheckBuyNowHandler> logger)
    {
        _userProfileRepository = userProfileRepository;
        _adjustmentHandle = adjustmentHandle;
        _productImageRepository = productImageRepository;
        _productRepository = productRepository;
        _paymentMethodRepository = paymentMethodRepository;
        _storeRepository = storeRepository;
        _walletRepository = walletRepository;
        _logger = logger;
    }

    public async Task<ServiceResult<CheckoutDto>> Handle(CheckBuyNowQuery request, CancellationToken cancellationToken)
    {
        // Step 1: Kiểm tra thông tin đầu vào
        var profile = await _userProfileRepository.GetByUserIdAsync(request.UserId);
        if (profile == null) return LogError<CheckoutDto>("Không tìm thấy thông tin người dùng.");

        var product = await _productRepository.GetByIdAsync(request.ProductId);
        if (product == null) return LogError<CheckoutDto>("Không tìm thấy sản phẩm.");

        var image = await _productImageRepository.GetPrimaryAsync(request.ProductId);
        if (image == null) return LogError<CheckoutDto>("Không tìm thấy ảnh đại diện sản phẩm.");

        var store = await _storeRepository.GetByIdAsync(product.StoreId);
        if (store == null) return LogError<CheckoutDto>("Không tìm thấy cửa hàng.");

        // Step 2: Tính giá giảm theo sản phẩm
        var productDiscount = await _adjustmentHandle.HandleDiscountAndPriceForProduct(product);
        if (productDiscount.Status == StatusResult.Errored)
            return LogError<CheckoutDto>("Lỗi khi tính giá sau giảm cho sản phẩm.");

        var finalPrice = productDiscount.Data.FinalAmount;
        var quantity = request.Quantity;
        var totalProductAmount = quantity * finalPrice;

        var checkout = new CheckoutDto
        {
            Items = new List<CheckoutItemDto>
            {
                new CheckoutItemDto
                {
                    ProductId = request.ProductId,
                    ProductName = product.ProductName,
                    StoreName = store.StoreName,
                    Price = finalPrice,
                    ProductImage = image.ImagePath,
                    Amount = totalProductAmount
                }
            }
        };

        // Step 3: Tính giảm giá theo đơn hàng (mock order)
        var mockOrder = new Order
        {
            OrderId = Guid.NewGuid().ToString(),
            BuyerId = request.UserId,
            IsPaid = false,
            TotalAmount = finalPrice,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow
        };

        var orderDiscount = await _adjustmentHandle.HandleDiscountForOrderAndPayment(order:mockOrder);
        if (orderDiscount.Status == StatusResult.Errored)
            return LogError<CheckoutDto>("Lỗi khi tính giảm giá theo đơn hàng.");

        checkout.Summary = new CheckoutSummaryDto
        {
            DiscountId = orderDiscount.Data.AdjustmentRuleId,
            FinalAmount = orderDiscount.Data.FinalAmount,
            Value = orderDiscount.Data.Value
        };

        // Step 4: Gợi ý phương thức thanh toán
        var wallet = await _walletRepository.GetByUserIdAsync(request.UserId);
        if (wallet != null)
        {
            checkout.PaymentMethods.Add(new PaymentMethodOptionDto
            {
                PaymentMethodId = wallet.WalletId,
                IsRecommended = true,
                AvailableBalance = wallet.Balance,
                MethodName = "Ví sàn",
                MethodCode = "wallet",
                Description = ""
            });
        }

        var externalMethods = await _paymentMethodRepository.GetByUserIdAsync(request.UserId);
        foreach (var method in externalMethods)
        {
            checkout.PaymentMethods.Add(new PaymentMethodOptionDto
            {
                PaymentMethodId = method.PaymentMethodId,
                IsRecommended = true,
                MethodName = "Phương thức khác",
                MethodCode = method.Provider.ToString()
            });
        }

        return ServiceResult<CheckoutDto>.Success(checkout);
    }

    private ServiceResult<T> LogError<T>(string message)
    {
        _logger.LogError(message);
        return ServiceResult<T>.Error(message);
    }
}
