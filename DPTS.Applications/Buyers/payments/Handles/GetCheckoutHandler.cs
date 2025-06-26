using DPTS.Applications.Buyers.payments.Dtos;
using DPTS.Applications.Buyers.payments.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyers.payments.Handles
{
    public class GetCheckoutHandler : IRequestHandler<GetCheckoutQuery, ServiceResult<CheckoutDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<GetCheckoutHandler> _logger;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITaxRepository _taxRepository;
        private readonly ILogRepository _logRepository;

        public GetCheckoutHandler(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            ILogger<GetCheckoutHandler> logger,
            ICategoryRepository categoryRepository,
            ITaxRepository taxRepository,
            ILogRepository logRepository)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _logger = logger;
            _categoryRepository = categoryRepository;
            _taxRepository = taxRepository;
            _logRepository = logRepository;
        }

        public async Task<ServiceResult<CheckoutDto>> Handle(GetCheckoutQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Bắt đầu xử lý GetCheckout cho buyerId: {BuyerId}", request.BuyerId);

            try
            {
                var orders = (await _orderRepository.GetsAsync(includeOrderItems: true))
                    .Where(x => x.IsPaid == false && x.BuyerId == request.BuyerId)
                    .ToList();

                if (!orders.Any() || orders.Count > 1)
                {
                    _logger.LogWarning("Không tìm thấy hoặc có nhiều hơn một đơn chưa thanh toán cho buyerId: {BuyerId}", request.BuyerId);
                    return ServiceResult<CheckoutDto>.Error("Không thể xử lý đơn hàng.");
                }

                var order = orders.Single();
                var products = await _productRepository.GetsAsync(includeStore: true, includeImages: true);
                var images = products.SelectMany(x => x.Images).Where(x => x.IsPrimary).ToList();
                var taxs = await _taxRepository.GetsAsync();
                var taxSystem = taxs.FirstOrDefault(x => x.CategoryId == string.Empty && x.TaxStatus == TaxStatus.Active);

                if (taxSystem == null)
                {
                    _logger.LogError("Không tìm thấy cấu hình thuế hệ thống.");
                    return ServiceResult<CheckoutDto>.Error("Cấu hình thuế không hợp lệ.");
                }

                var resultQuery = (
                    from item in order.OrderItems
                    join product in products on item.ProductId equals product.ProductId
                    select new
                    {
                        product.ProductId,
                        product.ProductName,
                        product.Price,
                        item.Quantity,
                        Amount = item.Quantity * product.Price,
                        ProductImage = images.FirstOrDefault(i => i.ProductId == product.ProductId)?.ImagePath ?? string.Empty,
                        StoreName = product.Store?.StoreName ?? "Không rõ"
                    }).ToList();

                var items = resultQuery.Select(x => new CheckoutItemDto
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    Price = x.Price,
                    ProductImage = x.ProductImage,
                    StoreName = x.StoreName,
                    Amount = x.Amount
                }).ToList();

                var subTotal = items.Sum(x => x.Amount);
                var total = subTotal + subTotal * taxSystem.Rate / 100;

                var summary = new CheckoutSummaryDto
                {
                    Subtotal = subTotal,
                    PlatformFee = taxSystem.Rate,
                    Tax = 0, // giữ nguyên nếu chưa tách loại thuế riêng
                    Total = total
                };

                try
                {
                    order.TotalAmount = total;
                    order.UpdatedAt = DateTime.UtcNow;
                    await _orderRepository.UpdateAsync(order);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Cập nhật đơn hàng thất bại cho orderId: {OrderId}", order.OrderId);
                    return ServiceResult<CheckoutDto>.Error("Cập nhật đơn hàng thất bại.");
                }

                try
                {
                    await _logRepository.AddAsync(new Log
                    {
                        LogId = Guid.NewGuid().ToString(),
                        Action = $"[{request.BuyerId}] cập nhật giỏ hàng cá nhân.",
                        CreatedAt = DateTime.UtcNow,
                        UserId = request.BuyerId
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Ghi log thất bại cho buyerId: {BuyerId}", request.BuyerId);
                    // Không return lỗi vì không ảnh hưởng logic chính
                }

                var result = new CheckoutDto
                {
                    Items = items,
                    Summary = summary
                };

                _logger.LogInformation("Hoàn tất xử lý GetCheckout cho buyerId: {BuyerId}", request.BuyerId);
                return ServiceResult<CheckoutDto>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý GetCheckout cho buyerId: {BuyerId}", request.BuyerId);
                return ServiceResult<CheckoutDto>.Error("Có lỗi xảy ra khi xử lý thanh toán.");
            }
        }
    }
}
