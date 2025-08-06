using DPTS.Applications.Buyer.Queries.order;
using DPTS.Applications.Buyer.Queries.payment;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace DPTS.Applications.Buyer.Handles.payment
{
    public class HandlePaymentCallbackHandler : IRequestHandler<HandlePaymentCallbackQuery, ServiceResult<string>>
    {
        private readonly IConfiguration _config;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<HandlePaymentCallbackHandler> _logger;
        private readonly IEscrowProcessRepository _escrowProcessRepository;
        private readonly IEscrowRepository _escrowRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMediator _mediator;
        private readonly IAdjustmentHandle _adjustmentHandle;

        public HandlePaymentCallbackHandler(IConfiguration config,
                                            IOrderRepository orderRepository,
                                            ILogger<HandlePaymentCallbackHandler> logger,
                                            IEscrowProcessRepository escrowProcessRepository,
                                            IEscrowRepository escrowRepository,
                                            IProductRepository productRepository,
                                            IMediator mediator,
                                            IAdjustmentHandle adjustmentHandle)
        {
            _config = config;
            _orderRepository = orderRepository;
            _logger = logger;
            _escrowProcessRepository = escrowProcessRepository;
            _escrowRepository = escrowRepository;
            _productRepository = productRepository;
            _mediator = mediator;
            _adjustmentHandle = adjustmentHandle;
        }

        public async Task<ServiceResult<string>> Handle(HandlePaymentCallbackQuery request, CancellationToken cancellationToken)
        {
            var queryParams = request.QueryData;

            if (!queryParams.TryGetValue("vnp_SecureHash", out var vnp_SecureHash))
                return ServiceResult<string>.Error("Thiếu chữ ký xác thực.");

            // Xoá 2 trường không dùng trong ký
            queryParams.Remove("vnp_SecureHash");
            queryParams.Remove("vnp_SecureHashType");

            // Tạo rawData theo định dạng chuẩn
            var rawData = string.Join("&", queryParams
                .OrderBy(p => p.Key)
                .Select(kv => $"{HttpUtility.UrlEncode(kv.Key)}={HttpUtility.UrlEncode(kv.Value)}"));

            var hashSecret = _config["Vnpay:HashSecret"];
            var computedHash = HmacSHA512(hashSecret, rawData);

            if (!string.Equals(computedHash, vnp_SecureHash, StringComparison.OrdinalIgnoreCase))
            {
                _logger.LogWarning("Sai chữ ký callback từ VNPAY.");
                return ServiceResult<string>.Error("Chữ ký không hợp lệ.");
            }

            // Đọc mã giao dịch và trạng thái
            var responseCode = queryParams.GetValueOrDefault("vnp_ResponseCode");
            var transactionStatus = queryParams.GetValueOrDefault("vnp_TransactionStatus");
            var orderCode = queryParams.GetValueOrDefault("vnp_TxnRef");
            var orderInfo = queryParams.GetValueOrDefault("vnp_OrderInfo");
            var splited = orderInfo.Split(' ');
            if (responseCode == "00" && transactionStatus == "00")
            {
                // Cập nhật trạng thái đơn hàng
                var order = await _orderRepository.GetByIdAsync(splited[4]);
                if(order == null)
                {
                    _logger.LogWarning($"Không tìm thấy đơn hàng với mã {orderCode}.");
                    return ServiceResult<string>.Error("Đơn hàng không tồn tại.");
                }
                var checkoutQuery = new GetCheckoutQuery { BuyerId = order.BuyerId };
                var checkoutResult = await _mediator.Send(checkoutQuery);
                if (checkoutResult.Status == StatusResult.Errored)
                {
                    _logger.LogError("Lỗi khi lấy dữ liệu checkout.");
                    return ServiceResult<string>.Error("Không thể lấy thông tin thanh toán.");
                }
                var items = checkoutResult.Data.Items;
                var products = await _productRepository.SearchAsync();
                var joinedItems = from item in items
                                  join product in products on item.ProductId equals product.ProductId
                                  select new JoinedResult
                                  {
                                      ProductId = item.ProductId,
                                      Amount = item.Amount,
                                      Price = item.Price,
                                      StoreId = product.StoreId
                                  };

                var escrows = new List<Escrow>();
                foreach (var item in joinedItems)
                {
                    var existing = escrows.FirstOrDefault(x => x.StoreId == item.StoreId);
                    if (existing == null)
                    {
                        escrows.Add(new Escrow
                        {
                            EscrowId = Guid.NewGuid().ToString(),
                            StoreId = item.StoreId,
                            OrderId = order.OrderId,
                            Amount = item.Amount,
                            CreatedAt = DateTime.UtcNow,
                            Expired = DateTime.UtcNow.AddDays(30),
                            ReleasedAt = DateTime.UtcNow,
                            ReleasedBy = "System",
                            UpdatedAt = DateTime.UtcNow
                        });
                    }
                    else
                    {
                        existing.Amount += item.Amount;
                        existing.UpdatedAt = DateTime.UtcNow;
                    }
                }

                // 6. Áp dụng platform fee và tax cho từng escrow
                foreach (var escrow in escrows)
                {
                    var feeResult = await _adjustmentHandle.HandlePlatformFeeForSeller(escrow.Amount);
                    if (feeResult.Status == StatusResult.Errored)
                    {
                        _logger.LogError("Lỗi khi tính phí nền tảng.");
                        return ServiceResult<string>.Error("Không thể tính phí nền tảng.");
                    }

                    var taxResult = await _adjustmentHandle.HandleTaxForSeller(feeResult.Data.FinalAmount);
                    if (taxResult.Status == StatusResult.Errored)
                    {
                        _logger.LogError("Lỗi khi tính thuế.");
                        return ServiceResult<string>.Error("Không thể tính thuế.");
                    }

                    escrow.PlatformFeeRate = feeResult.Data.Value;
                    escrow.PlatformFeeAmount = feeResult.Data.Value;
                    escrow.TaxRate = taxResult.Data.Value;
                    escrow.TaxAmount = taxResult.Data.FinalAmount;
                    escrow.ActualAmount = escrow.Amount - escrow.PlatformFeeAmount - escrow.TaxAmount;
                    escrow.UpdatedAt = DateTime.UtcNow;
                }
                var log = new Log()
                {
                    Action = "PaymentCallback",
                    CreatedAt = DateTime.UtcNow,
                    LogId = Guid.NewGuid().ToString(),
                    UserId = order.BuyerId,

                    Description = "Thanh toán thành công qua ví."
                };
                try
                {
                    foreach (var item in escrows)
                    {
                        var addProcess = new EscrowProcess()
                        {
                            EscrowId = item.EscrowId,
                            ProcessName = "Đã thanh toán",
                            ProcessId = Guid.NewGuid().ToString(),
                            ProcessAt = DateTime.UtcNow
                        };
                        await _escrowRepository.AddAsync(item);
                        await _escrowProcessRepository.AddAsync(addProcess);
                    }

                    // 9. Đánh dấu thanh toán
                    order.IsPaid = true;
                    order.UpdatedAt = DateTime.UtcNow;
                    await _orderRepository.UpdateAsync(order);
                    return ServiceResult<string>.Success("Thanh toán thành công.");
                }
                catch(Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi cập nhật thông tin thanh toán.");
                    return ServiceResult<string>.Error("Lỗi khi cập nhật thông tin thanh toán.");
                }

                
            }

            return ServiceResult<string>.Error("Thanh toán thất bại hoặc bị hủy.");
        }

        private string HmacSHA512(string key, string input)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            using var hmac = new HMACSHA512(keyBytes);
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(input));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToUpperInvariant();
        }
        private class JoinedResult
        {
            public string ProductId { get; set; } = string.Empty;
            public string StoreId { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public decimal Amount { get; set; }
        }
    }
}
