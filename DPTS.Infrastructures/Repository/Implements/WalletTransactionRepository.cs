using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Transactions;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class WalletTransactionRepository : IWalletTransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public WalletTransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(WalletTransaction transaction)
        {
            _context.WalletTransactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(WalletTransaction transaction)
        {
            _context.WalletTransactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string transactionId)
        {
            var entity = await _context.WalletTransactions.FindAsync(transactionId);
            if (entity == null) return;
            _context.WalletTransactions.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<WalletTransaction?> GetByIdAsync(string transactionId)
        {
            if (string.IsNullOrWhiteSpace(transactionId)) return null;

            return await _context.WalletTransactions
                .AsNoTracking()
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
        }

        public async Task<IEnumerable<WalletTransaction>> GetsAsync(
            string? walletId = null,
            TransactionType? type = null,
            TransactionStatus? status = null,
            DateTime? from = null,
            DateTime? to = null,
            decimal? minAmount = null,
            decimal? maxAmount = null)
        {
            var query = _context.WalletTransactions.AsQueryable();

            if (!string.IsNullOrWhiteSpace(walletId))
                query = query.Where(t => t.WalletId == walletId);

            if (type.HasValue)
                query = query.Where(t => t.Type == type);

            if (status.HasValue)
                query = query.Where(t => t.Status == status);

            if (from.HasValue)
                query = query.Where(t => t.Timestamp >= from);

            if (to.HasValue)
                query = query.Where(t => t.Timestamp <= to);

            if (minAmount.HasValue)
                query = query.Where(t => t.Amount >= minAmount);

            if (maxAmount.HasValue)
                query = query.Where(t => t.Amount <= maxAmount);

            return await query
                .OrderByDescending(t => t.Timestamp)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}