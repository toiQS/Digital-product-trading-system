using DPTS.Applications.Dtos;
using DPTS.Applications.Interfaces;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Implements
{
    public class WalletService : IWalletService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<WalletService> _logger;
        public WalletService(ApplicationDbContext context, ILogger<WalletService> logger)
        {
            _context = context;
            _logger = logger;
        }
        
        public async Task<ServiceResult<WalletPersonalDto>> WalletPersonalAsync(string userId)
        {
            _logger.LogInformation("");
            var wallet = await _context.Wallets.Where(x => x.UserId == userId).FirstOrDefaultAsync();
            if (wallet == null) return ServiceResult<WalletPersonalDto>.Error("");
            WalletPersonalDto walletDto = new WalletPersonalDto()
            {
                WalletId = wallet.UserId,
                AvaibableBalance = wallet.AvaibableBalance,
                UnitCurrency = EnumHandle.HandleWalletUnitCurrency(wallet.Currency),
                UserId = userId,
            };
            var trades = await _context.Trades.Where(x => x.TradeFromId == wallet.WalletId || x.TradeToId == wallet.WalletId).ToListAsync();
            if(!trades.Any()) return ServiceResult<WalletPersonalDto>.Success(walletDto);
            walletDto.IndexTradeHistories = trades.Select(x => new IndexTradeHistoryDto()
            {
                TradeIcon = x.TradeIcon,
                TradeName = x.TradeName,
                Amount = x.Amount,
                Status = EnumHandle.HandleTradeStatus(x.Status),
                TradeAt = x.TradeDate.ToString("dd/mm/yyyy-HH:mm")
            }).ToList();
            return ServiceResult<WalletPersonalDto>.Success(walletDto);
        }
    }
}
