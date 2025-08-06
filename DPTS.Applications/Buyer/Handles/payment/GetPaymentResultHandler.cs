using DPTS.Applications.Buyer.Queries.order;
using DPTS.Applications.Buyer.Queries.payment;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MailKit.Search;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace DPTS.Applications.Buyer.Handles.payment
{
    public class GetPaymentResultHandler : IRequestHandler<GetPaymentResultQuery, ServiceResult<string>>
    {
        private readonly IAdjustmentHandle _adjustmentHandle;
        private readonly IWalletRepository _walletRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IEscrowRepository _escrowRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly ILogRepository _logRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMediator _mediator;
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly IEscrowProcessRepository _escrowProcessRepository;
        private readonly IOrderPaymentRepository _orderPaymentRepository;
        private readonly IConfiguration _config;
        private readonly ILogger<GetPaymentResultHandler> _logger;

        public GetPaymentResultHandler(IAdjustmentHandle adjustmentHandle,
                                       IWalletRepository walletRepository,
                                       IUserProfileRepository userProfileRepository,
                                       IOrderRepository orderRepository,
                                       IEscrowRepository escrowRepository,
                                       IPaymentMethodRepository paymentMethodRepository,
                                       ILogRepository logRepository,
                                       IProductRepository productRepository,
                                       IMediator mediator,
                                       IWalletTransactionRepository walletTransactionRepository,
                                       IEscrowProcessRepository escrowProcessRepository,
                                       IOrderPaymentRepository orderPaymentRepository,
                                       IConfiguration config,
                                       ILogger<GetPaymentResultHandler> logger)
        {
            _adjustmentHandle = adjustmentHandle;
            _walletRepository = walletRepository;
            _userProfileRepository = userProfileRepository;
            _orderRepository = orderRepository;
            _escrowRepository = escrowRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _logRepository = logRepository;
            _productRepository = productRepository;
            _mediator = mediator;
            _walletTransactionRepository = walletTransactionRepository;
            _escrowProcessRepository = escrowProcessRepository;
            _orderPaymentRepository = orderPaymentRepository;
            _config = config;
            _logger = logger;
        }

        public async Task<ServiceResult<string>> Handle(GetPaymentResultQuery request, CancellationToken cancellationToken)
        {

            // 1. Lấy thông tin người dùng
            var user = await _userProfileRepository.GetByUserIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("Không tìm thấy người dùng.");
                return ServiceResult<string>.Error("Không tìm thấy người dùng.");
            }

            // 2. Gửi query lấy dữ liệu checkout
            var checkoutQuery = new GetCheckoutQuery { BuyerId = request.UserId };
            var checkoutResult = await _mediator.Send(checkoutQuery);
            if (checkoutResult.Status == StatusResult.Errored)
            {
                _logger.LogError("Lỗi khi lấy dữ liệu checkout.");
                return ServiceResult<string>.Error("Không thể lấy thông tin thanh toán.");
            }

            // 3. Lấy đơn hàng chưa thanh toán của người dùng
            var unpaidOrders = (await _orderRepository.GetByBuyerAsync(request.UserId))
                .Where(x => !x.IsPaid)
                .ToList();

            if (!unpaidOrders.Any())
            {
                _logger.LogError("Không tìm thấy đơn hàng chưa thanh toán.");
                return ServiceResult<string>.Error("Bạn không có đơn hàng nào cần thanh toán.");
            }

            if (unpaidOrders.Count > 1)
            {
                _logger.LogError("Tìm thấy nhiều hơn một đơn hàng chưa thanh toán.");
                return ServiceResult<string>.Error("Hệ thống phát hiện nhiều đơn hàng chưa thanh toán. Vui lòng liên hệ hỗ trợ.");
            }

            var order = unpaidOrders.First();

            // 4. Mapping item trong đơn hàng với sản phẩm tương ứng
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

            // 5. Gom nhóm theo storeId để tạo escrow cho từng seller
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

            // 7. Xác định phương thức thanh toán
            var wallet = await _walletRepository.GetByIdAsync(request.PaymentMethodId);
            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(request.PaymentMethodId);

            if (wallet == null && paymentMethod == null)
            {
                _logger.LogError("Không tìm thấy phương thức thanh toán.");
                return ServiceResult<string>.Error("Phương thức thanh toán không hợp lệ.");
            }
            string result = string.Empty;

            // 8. Thực hiện thanh toán
            try
            {
                

                // 10. Trừ tiền ví nếu có
                if (wallet != null && wallet.Balance >= order.TotalAmount)
                {
                    // 8. Ghi escrow
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
                    WalletTransaction walletTransaction = new WalletTransaction()
                    {
                        TransactionId = Guid.NewGuid().ToString(),
                        Description = $"Thanh toán giỏ hàng {order.OrderId}",
                        Amount = order.TotalAmount,
                        WalletId = wallet.WalletId,
                        Status = WalletTransactionStatus.Pending,
                        Type = TransactionType.Purchase,
                        Timestamp = DateTime.UtcNow,
                    };
                    var orderPayment = new OrderPayment()
                    {
                        OrderId = order.OrderId,
                        Amount = order.TotalAmount,
                        OrderPaymentId = Guid.NewGuid().ToString(),
                        PaidAt = DateTime.UtcNow,
                        PaymentMethodId = request.PaymentMethodId,
                        SourceType = PaymentSourceType.Wallet,
                        WalletId = wallet.WalletId,
                    };
                    wallet.Balance -= order.TotalAmount;
                    // 11. Ghi log
                    var log = new Log
                    {
                        UserId = user.UserId,
                        Action = "PaymentSuccess",
                        CreatedAt = DateTime.UtcNow,
                        LogId = Guid.NewGuid().ToString(),
                        TargetId = order.OrderId,
                        TargetType = "Order"
                    };
                    await _logRepository.AddAsync(log);
                    await _walletTransactionRepository.AddAsync(walletTransaction);
                    await _walletRepository.UpdateAsync(wallet);
                    await _orderPaymentRepository.AddAsync(orderPayment);
                    result = "Thanh toán thành công qua ví.";
                }
                else if (paymentMethod != null)
                {
                    var paymentMethodInfo = await _paymentMethodRepository.GetByIdAsync(paymentMethod.PaymentMethodId);
                    if (paymentMethodInfo.Provider == PaymentProvider.VnPay)
                    {
                        var vnp_TmnCode = _config["Vnpay:TmnCode"];
                        var vnp_HashSecret = _config["Vnpay:HashSecret"];
                        var vnp_Url = _config["Vnpay:PaymentUrl"];
                        var vnp_Returnurl = _config["Vnpay:ReturnUrl"];

                        var vnp_TxnRef = DateTime.UtcNow.Ticks.ToString();
                        var vnp_Amount = ((int)(order.TotalAmount * 100)).ToString();

                        var inputData = new Dictionary<string, string>
                        {
                            {"vnp_Version", "2.1.0"},
                            {"vnp_Command", "pay"},
                            {"vnp_TmnCode", vnp_TmnCode},
                            {"vnp_Amount", vnp_Amount},
                            {"vnp_CreateDate", DateTime.UtcNow.ToString("yyyyMMddHHmmss")},
                            {"vnp_CurrCode", "VND"},
                            {"vnp_IpAddr", request.IpAddress},
                            {"vnp_Locale", "vn"},
                            {"vnp_OrderInfo", $"Thanh toan don hang {order.OrderId}"},
                            {"vnp_OrderType", "other"},
                            {"vnp_ReturnUrl", vnp_Returnurl},
                            {"vnp_TxnRef", vnp_TxnRef}
                        };
                        var paymentUrl = VnpayHelper.CreateRequestUrl(inputData, vnp_HashSecret, vnp_Url);
                        var log = new Log
                        {
                            UserId = user.UserId,
                            Action = "PaymentInitiated",
                            CreatedAt = DateTime.UtcNow,
                            LogId = Guid.NewGuid().ToString(),
                            TargetId = order.OrderId,
                            TargetType = "Order"
                        };
                        await _logRepository.AddAsync(log);
                        result = "Vui lòng thanh toán tại: " + paymentUrl;
                    }
                    else
                    {
                        return ServiceResult<string>.Error("Cổng thanh toán chưa được hỗ trợ.");
                    }
                }
                return ServiceResult<string>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý thanh toán.");
                return ServiceResult<string>.Error("Thanh toán thất bại. Vui lòng thử lại sau.");
            }

        }

        /// <summary>
        /// Kết quả kết hợp giữa item trong giỏ hàng và thông tin sản phẩm
        /// </summary>
        private class JoinedResult
        {
            public string ProductId { get; set; } = string.Empty;
            public string StoreId { get; set; } = string.Empty;
            public decimal Price { get; set; }
            public decimal Amount { get; set; }
        }
    }
    public static class VnpayHelper
    {
        public static string CreateRequestUrl(Dictionary<string, string> inputData, string secret, string baseUrl)
        {
            var ordered = inputData.OrderBy(k => k.Key);
            var rawData = BuildRawData(ordered);

            var secureHash = HmacSHA512(secret, rawData);

            inputData["vnp_SecureHashType"] = "HMACSHA512";
            inputData["vnp_SecureHash"] = secureHash;

            var queryString = string.Join("&", inputData.OrderBy(k => k.Key).Select(kvp =>
                $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));

            return $"{baseUrl}?{queryString}";
        }

        public static string BuildRawData(IEnumerable<KeyValuePair<string, string>> data)
        {
            return string.Join("&", data.Select(kvp =>
                $"{WebUtility.UrlEncode(kvp.Key)}={WebUtility.UrlEncode(kvp.Value)}"));
        }

        public static string BuildRawData(Dictionary<string, string> data)
        {
            return BuildRawData(data.OrderBy(k => k.Key));
        }

        public static string HmacSHA512(string key, string inputData)
        {
            var keyBytes = Encoding.UTF8.GetBytes(key);
            using var hmac = new HMACSHA512(keyBytes);
            var hashBytes = hmac.ComputeHash(Encoding.UTF8.GetBytes(inputData));
            return BitConverter.ToString(hashBytes).Replace("-", "").ToUpperInvariant();
        }
    }
}
