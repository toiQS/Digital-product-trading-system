using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class WalletTransactionRepository : IWalletTransactionRepository
    {
        private readonly ApplicationDbContext _context;

        public WalletTransactionRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Read

        public async Task<WalletTransaction?> GetByIdAsync(string transactionId)
        {
            return await _context.WalletTransactions
                .Include(t => t.LinkedPaymentMethod)
                .FirstOrDefaultAsync(t => t.TransactionId == transactionId);
        }

        public async Task<IEnumerable<WalletTransaction>> GetByWalletIdAsync(string walletId, bool includePaymentMethod = false)
        {
            var query = _context.WalletTransactions
                .Where(t => t.WalletId == walletId)
                .OrderByDescending(t => t.Timestamp)
                .AsQueryable();

            if (includePaymentMethod)
                query = query.Include(t => t.LinkedPaymentMethod);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<WalletTransaction>> GetByWalletIdAndTypeAsync(string walletId, TransactionType type)
        {
            return await _context.WalletTransactions
                .Where(t => t.WalletId == walletId && t.Type == type)
                .OrderByDescending(t => t.Timestamp)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(string transactionId)
        {
            return await _context.WalletTransactions.AnyAsync(t => t.TransactionId == transactionId);
        }

        #endregion

        #region Create

        public async Task AddAsync(WalletTransaction transaction)
        {
            _context.WalletTransactions.Add(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task AddManyAsync(IEnumerable<WalletTransaction> transactions)
        {
            _context.WalletTransactions.AddRange(transactions);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Update

        public async Task UpdateAsync(WalletTransaction transaction)
        {
            _context.WalletTransactions.Update(transaction);
            await _context.SaveChangesAsync();
        }

        public async Task<PaymentMethod?> GetPaymentMethodAsync(string? paymentMethodId)
        {
            return await _context.PaymentMethods
                .FirstOrDefaultAsync(pm => pm.PaymentMethodId == paymentMethodId) 
                ?? null;
        }

        #endregion
    }
}
