using DPTS.Applications.Buyer.Dtos.wallet;
using DPTS.Applications.Buyer.Queries.wallet;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles.wallet
{
    public class GetWalletHandle : IRequestHandler<GetWalletQuery, ServiceResult<WalletDto>>
    {
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IWalletTransactionRepository _walletTransactionRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly ILogRepository _logRepository;
        private readonly ILogger<GetWalletHandle> _logger;

        public GetWalletHandle(
            IUserProfileRepository userProfileRepository,
            IWalletRepository walletRepository,
            IWalletTransactionRepository walletTransactionRepository,
            IPaymentMethodRepository paymentMethodRepository,
            ILogRepository logRepository,
            ILogger<GetWalletHandle> logger)
        {
            _userProfileRepository = userProfileRepository;
            _walletRepository = walletRepository;
            _walletTransactionRepository = walletTransactionRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _logRepository = logRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<WalletDto>> Handle(GetWalletQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling wallet of user");
            // 1. Kiểm tra thông tin người dùng
            var profile = await _userProfileRepository.GetByUserIdAsync(request.UserId);
            if (profile == null)
            {
                _logger.LogError("User not found. UserId={UserId}", request.UserId);
                return ServiceResult<WalletDto>.Error("Không tìm thấy người dùng.");
            }

            // 2. Lấy ví của người dùng
            var wallet = await _walletRepository.GetByUserIdAsync(request.UserId);
            if (wallet == null)
            {
                _logger.LogError("Wallet not found for user. UserId={UserId}", request.UserId);
                return ServiceResult<WalletDto>.Error("Không tìm thấy ví.");
            }

            // 3. Lấy danh sách giao dịch và tính số dư
            var trades = await _walletTransactionRepository.GetByWalletIdAsync(wallet.WalletId);

            var moneyIn = trades
                .Where(x => x.Type == Domains.TransactionType.Deposit || x.Type == Domains.TransactionType.Refund)
                .Sum(x => x.Amount);

            var moneyOut = trades
                .Where(x => x.Type == Domains.TransactionType.Purchase || x.Type == Domains.TransactionType.Withdraw)
                .Sum(x => x.Amount);

            if (moneyOut > moneyIn)
            {
                _logger.LogError("Inconsistent wallet balance. WalletId={WalletId}, MoneyIn={MoneyIn}, MoneyOut={MoneyOut}",
                    wallet.WalletId, moneyIn, moneyOut);
                return ServiceResult<WalletDto>.Error("Dữ liệu ví không hợp lệ.");
            }

            try
            {
                // 4. Cập nhật số dư và ghi log truy cập ví
                wallet.Balance = moneyIn - moneyOut;
                await _walletRepository.UpdateAsync(wallet);

                var log = new Log
                {
                    LogId = Guid.NewGuid().ToString(),
                    UserId = request.UserId,
                    Action = "GetWallet",
                    TargetId = wallet.WalletId,
                    TargetType = "Wallet",
                    CreatedAt = DateTime.UtcNow
                };
                await _logRepository.AddAsync(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update wallet or write log. WalletId={WalletId}", wallet.WalletId);
                return ServiceResult<WalletDto>.Error("Không thể cập nhật ví hoặc ghi log.");
            }

            // 5. Chuẩn bị dữ liệu kết quả trả về
            var result = new WalletDto
            {
                WalletId = wallet.WalletId,
                Balance = wallet.Balance,
                Trades = trades.Select(x => new TradeDto
                {
                    TradeId = x.TransactionId,
                    Amount = x.Amount,
                    Status = EnumHandle.HandleWalletTransactionStatus(x.Status),
                    TradeAt = x.Timestamp.ToString("dd/MM/yyyy - HH:mm")
                }).ToList()
            };

            // 6. Lấy danh sách phương thức thanh toán
            var paymentMethods = await _paymentMethodRepository.GetByUserIdAsync(wallet.UserId);
            result.PaymentMethods = paymentMethods.Select(x => new PaymentMethodDto
            {
                PaymentMethodId = x.PaymentMethodId,
                MaskedAccountNumber = x.MaskedAccountNumber ?? "Không xác định",
                PaymentMethodName = EnumHandle.HandlePaymentProvider(x.Provider)
            }).ToList();

            return ServiceResult<WalletDto>.Success(result);
        }
    }
}
