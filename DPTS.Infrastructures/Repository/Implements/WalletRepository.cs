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

        #region Read

        // Lấy ví theo WalletId, có tùy chọn include danh sách giao dịch
        public async Task<Wallet?> GetByIdAsync(string walletId, bool includeTransactions = false)
        {
            if (string.IsNullOrWhiteSpace(walletId))
                return null;

            var query = _context.Wallets.AsQueryable();

            if (includeTransactions)
                query = query.Include(w => w.Transactions);

            return await query.FirstOrDefaultAsync(w => w.WalletId == walletId);
        }

        // Lấy ví theo UserId
        public async Task<Wallet?> GetByUserIdAsync(string userId, bool includeTransactions = false)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return null;

            var query = _context.Wallets.AsQueryable();

            if (includeTransactions)
                query = query.Include(w => w.Transactions);

            return await query.FirstOrDefaultAsync(w => w.UserId == userId);
        }

        // Kiểm tra sự tồn tại của ví cho một User
        public async Task<bool> ExistsByUserIdAsync(string userId)
        {
            return await _context.Wallets.AnyAsync(w => w.UserId == userId);
        }

        #endregion

        #region Create / Update / Delete

        // Tạo mới một ví
        public async Task AddAsync(Wallet wallet)
        {
            _context.Wallets.Add(wallet);
            await _context.SaveChangesAsync();
        }

        // Cập nhật thông tin ví, ví dụ: số dư
        public async Task UpdateAsync(Wallet wallet)
        {
            _context.Wallets.Update(wallet);
            await _context.SaveChangesAsync();
        }

        // Xoá ví theo WalletId
        public async Task DeleteAsync(string walletId)
        {
            var wallet = await _context.Wallets.FindAsync(walletId);
            if (wallet != null)
            {
                _context.Wallets.Remove(wallet);
                await _context.SaveChangesAsync();
            }
        }

        #endregion

        #region Transaction Helpers (Nếu cần tích hợp thêm WalletTransaction)

        // Ví dụ: Thêm một giao dịch và cập nhật số dư
        public async Task AddTransactionAsync(string walletId, WalletTransaction transaction)
        {
            var wallet = await GetByIdAsync(walletId, includeTransactions: true);
            if (wallet == null)
                throw new ArgumentException("Wallet not found", nameof(walletId));

            // Thêm giao dịch vào danh sách
            wallet.Transactions.Add(transaction);
            // Cập nhật số dư
            wallet.Balance += transaction.Amount;

            await _context.SaveChangesAsync();
        }

        #endregion
    }
}
