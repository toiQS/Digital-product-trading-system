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

        public async Task AddAsync(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string walletId)
        {
            var entity = await _context.Wallets.FindAsync(walletId);
            if (entity == null) return;
            _context.Wallets.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<Wallet?> GetByIdAsync(string walletId)
        {
            if (string.IsNullOrWhiteSpace(walletId)) return null;

            return await _context.Wallets
                .AsNoTracking()
                .FirstOrDefaultAsync(w => w.WalletId == walletId);
        }

        public async Task<IEnumerable<Wallet>> GetsAsync(
            string? userId = null,
            decimal? minBalance = null,
            decimal? maxBalance = null)
        {
            var query = _context.Wallets.AsQueryable();

            if (!string.IsNullOrWhiteSpace(userId))
                query = query.Where(w => w.UserId == userId);

            if (minBalance.HasValue)
                query = query.Where(w => w.Balance >= minBalance);

            if (maxBalance.HasValue)
                query = query.Where(w => w.Balance <= maxBalance);

            return await query
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
