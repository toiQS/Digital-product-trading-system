using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class WalletRepository : IWalletRepository
    {
        private readonly ApplicationDbContext _context;

        public WalletRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Wallet?> GetByIdAsync(string walletId, bool includeUser = false)
        {
            if (string.IsNullOrWhiteSpace(walletId)) return null;

            var query = _context.Wallets.AsQueryable();

            if (includeUser)
                query = query.Include(w => w.User);

            return await query
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.WalletId == walletId);
        }

        public async Task<Wallet?> GetByUserIdAsync(string userId, bool includeUser = false)
        {
            if (string.IsNullOrWhiteSpace(userId)) return null;

            var query = _context.Wallets.AsQueryable();

            if (includeUser)
                query = query.Include(w => w.User);

            return await query
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.UserId == userId);
        }

        public async Task<IEnumerable<Wallet>> GetsAsync(
            UnitCurrency? currency = null,
            double? minBalance = null,
            double? maxBalance = null,
            string? userKeyword = null,
            bool includeUser = false)
        {
            var query = _context.Wallets.AsQueryable();

            if (currency.HasValue)
                query = query.Where(w => w.Currency == currency.Value);

            if (minBalance.HasValue)
                query = query.Where(w => w.AvaibableBalance >= minBalance.Value);

            if (maxBalance.HasValue)
                query = query.Where(w => w.AvaibableBalance <= maxBalance.Value);

            if (!string.IsNullOrWhiteSpace(userKeyword))
            {
                var lowered = userKeyword.ToLower();
                query = query.Where(w =>
                    w.User.Username.ToLower().Contains(lowered) ||
                    w.User.Email.ToLower().Contains(lowered));
            }

            if (includeUser)
                query = query.Include(w => w.User);

            return await query
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task AddAsync(Wallet wallet)
        {
            if (wallet == null) throw new ArgumentNullException(nameof(wallet));
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Wallet wallet)
        {
            if (wallet == null) throw new ArgumentNullException(nameof(wallet));
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(Wallet wallet)
        {
            if (wallet == null) return;
            _context.Wallets.Remove(wallet);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> ExistsAsync(string walletId)
        {
            if (string.IsNullOrWhiteSpace(walletId)) return false;
            return await _context.Wallets.AnyAsync(w => w.WalletId == walletId);
        }
    }
}
