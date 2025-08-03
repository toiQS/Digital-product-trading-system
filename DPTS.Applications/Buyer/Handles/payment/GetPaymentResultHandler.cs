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
                    await _walletTransactionRepository.AddAsync(walletTransaction);
                    await _walletRepository.UpdateAsync(wallet);
                    await _orderPaymentRepository.AddAsync(orderPayment);
                }
                else if (paymentMethod != null)
                {
                    var paymentMethodInfo = await _paymentMethodRepository.GetByIdAsync(paymentMethod.PaymentMethodId);
                    if (paymentMethodInfo.Provider == PaymentProvider.VnPay)
                    {
                        var vnpay = new VnPayLibrary();
                        string vnp_Url = _config["Vnpay:BaseUrl"];
                        string returnUrl = _config["Vnpay:ReturnUrl"];
                        string tmnCode = _config["Vnpay:TmnCode"];
                        string hashSecret = _config["Vnpay:HashSecret"];
                        string orderId = order.OrderId;
                        string amount = ((int)(order.TotalAmount * 100)).ToString(); // VNPAY yêu cầu đơn vị = VND * 100

                        vnpay.AddRequestData("vnp_Version", "2.1.0");
                        vnpay.AddRequestData("vnp_Command", "pay");
                        vnpay.AddRequestData("vnp_TmnCode", tmnCode);
                        vnpay.AddRequestData("vnp_Amount", amount);
                        vnpay.AddRequestData("vnp_CreateDate", DateTime.UtcNow.ToString("yyyyMMddHHmmss"));
                        vnpay.AddRequestData("vnp_CurrCode", "VND");
                        vnpay.AddRequestData("vnp_IpAddr", request.IpAddress);
                        vnpay.AddRequestData("vnp_Locale", "vn");
                        vnpay.AddRequestData("vnp_OrderInfo", $"Thanh toan don hang {orderId}");
                        vnpay.AddRequestData("vnp_OrderType", "other");
                        vnpay.AddRequestData("vnp_ReturnUrl", returnUrl);
                        vnpay.AddRequestData("vnp_TxnRef", orderId);
                        vnpay.AddRequestData("vnp_BankCode", paymentMethodInfo.MaskedAccountNumber);

                        string paymentUrl = vnpay.CreateRequestUrl(vnp_Url, hashSecret);

                        return ServiceResult<string>.Success(paymentUrl);
                    }
                    else
                    {
                        return ServiceResult<string>.Error("Cổng thanh toán chưa được hỗ trợ.");
                    }
                }


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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xử lý thanh toán.");
                return ServiceResult<string>.Error("Thanh toán thất bại. Vui lòng thử lại sau.");
            }

            return ServiceResult<string>.Success("Thanh toán thành công.");
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
    public class VnPayLibrary
    {
        private readonly SortedList<string, string> _requestData = new(new VnPayCompare());

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
            {
                _requestData[key] = value;
            }
        }

        public string CreateRequestUrl(string baseUrl, string hashSecret)
        {
            var data = new StringBuilder();
            var query = new StringBuilder();
            foreach (var kv in _requestData)
            {
                data.Append(kv.Key).Append('=').Append(kv.Value).Append('&');
                query.Append(HttpUtility.UrlEncode(kv.Key)).Append('=').Append(HttpUtility.UrlEncode(kv.Value)).Append('&');
            }

            // Remove last '&'
            if (data.Length > 0) data.Length -= 1;
            if (query.Length > 0) query.Length -= 1;

            string signData = data.ToString();
            string secureHash = ComputeSha256(hashSecret + signData);
            return $"{baseUrl}?{query}&vnp_SecureHashType=SHA256&vnp_SecureHash={secureHash}";
        }

        public static string ComputeSha256(string input)
        {
            using var sha256 = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(input);
            var hash = sha256.ComputeHash(bytes);
            return BitConverter.ToString(hash).Replace("-", "").ToLower();
        }

        private class VnPayCompare : IComparer<string>
        {
            public int Compare(string x, string y)
            {
                return string.CompareOrdinal(x, y);
            }
        }
    }
}
