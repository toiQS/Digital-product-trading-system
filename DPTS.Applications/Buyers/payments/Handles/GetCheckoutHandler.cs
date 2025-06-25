using DPTS.Applications.Buyers.payments.Dtos;
using DPTS.Applications.Buyers.payments.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyers.payments.Handles
{
    public class GetCheckoutHandler : IRequestHandler<GetCheckoutQuery, ServiceResult<CheckoutDto>>
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ITaxRepository _taxRepository;
        private readonly ILogger<GetCheckoutHandler> _logger;

        public GetCheckoutHandler(
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            ICategoryRepository categoryRepository,
            ITaxRepository taxRepository,
            ILogger<GetCheckoutHandler> logger)
        {
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _categoryRepository = categoryRepository;
            _taxRepository = taxRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<CheckoutDto>> Handle(GetCheckoutQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Bắt đầu xử lý GetCheckout cho Buyer");

            try
            {
                // Lấy đơn hàng chưa thanh toán
                var orders = (await _orderRepository.GetsAsync(includeOrderItems: true))
                    .Where(x => !x.IsPaid)
                    .ToList();

                if (orders.Count == 0)
                {
                    _logger.LogWarning("Không có đơn hàng chưa thanh toán.");
                    return ServiceResult<CheckoutDto>.Error("Không có đơn hàng chưa thanh toán.");
                }

                if (orders.Count > 1)
                {
                    _logger.LogError("Tồn tại nhiều đơn hàng chưa thanh toán.");
                    return ServiceResult<CheckoutDto>.Error("Dữ liệu đơn hàng không hợp lệ.");
                }

                var order = orders.Single();
                var items = order.OrderItems;

                // Lấy dữ liệu sản phẩm, thuế
                var products = await _productRepository.GetsAsync(includeStore: true, includeImages: true);
                var taxes = await _taxRepository.GetsAsync();

                var taxSystem = taxes.SingleOrDefault(x => x.CategoryId == string.Empty && x.TaxStatus == Domains.TaxStatus.Active);
                if (taxSystem == null)
                {
                    _logger.LogError("Không tìm thấy thuế hệ thống đang hoạt động.");
                    return ServiceResult<CheckoutDto>.Error("Không tìm thấy thuế hệ thống.");
                }

                var primaryImages = products
                    .SelectMany(p => p.Images)
                    .Where(i => i.IsPrimary)
                    .ToDictionary(i => i.ProductId, i => i.ImagePath);

                // Join dữ liệu
                List<QueryResult> queryResults;
                try
                {
                    queryResults = (
                        from item in items
                        join product in products on item.ProductId equals product.ProductId
                        select new QueryResult
                        {
                            ProductId = product.ProductId,
                            ProductName = product.ProductName,
                            Price = product.Price,
                            Quantity = item.Quantity,
                            Amount = product.Price * item.Quantity,
                            ProductImage = primaryImages.ContainsKey(product.ProductId)
                                ? primaryImages[product.ProductId]
                                : "/images/default-product.png",
                            StoreName = product.Store?.StoreName ?? "Không rõ"
                        }).ToList();

                    _logger.LogInformation("Đã xử lý {Count} sản phẩm trong giỏ hàng.", queryResults.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi join dữ liệu sản phẩm.");
                    return ServiceResult<CheckoutDto>.Error("Không thể xử lý dữ liệu giỏ hàng.");
                }

                // Mapping ra DTO
                var itemResult = queryResults.Select(x => new CheckoutItemDto
                {
                    ProductId = x.ProductId,
                    ProductName = x.ProductName,
                    Price = x.Price,
                    Amount = x.Amount,
                    ProductImage = x.ProductImage,
                    StoreName = x.StoreName
                }).ToList();

                var subTotal = itemResult.Sum(x => x.Amount);
                var taxRate = taxSystem.Rate;
                var total = subTotal + subTotal * taxRate / 100;

                var summary = new CheckoutSummaryDto
                {
                    Subtotal = subTotal,
                    PlatformFee = taxRate,
                    Tax = 0,
                    Total = total
                };

                var result = new CheckoutDto
                {
                    Items = itemResult,
                    Summary = summary
                };

                _logger.LogInformation("Trả về giỏ hàng cho buyer với tổng cộng {Count} sản phẩm.", itemResult.Count);
                return ServiceResult<CheckoutDto>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định trong GetCheckoutHandler.");
                return ServiceResult<CheckoutDto>.Error("Không thể lấy thông tin giỏ hàng.");
            }
        }

        private class QueryResult
        {
            public string ProductId { get; set; } = string.Empty;
            public string ProductName { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public int Quantity { get; set; }
            public decimal Amount { get; set; }
            public string ProductImage { get; set; } = string.Empty;
            public string StoreName { get; set; } = string.Empty;
        }
    }
}
