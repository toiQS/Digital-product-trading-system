using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IWalletTransactionRepository
    {
        Task AddAsync(WalletTransaction transaction);
        Task AddManyAsync(IEnumerable<WalletTransaction> transactions);
        Task<bool> ExistsAsync(string transactionId);
        Task<WalletTransaction?> GetByIdAsync(string transactionId);
        Task<IEnumerable<WalletTransaction>> GetByWalletIdAndTypeAsync(string walletId, TransactionType type);
        Task<IEnumerable<WalletTransaction>> GetByWalletIdAsync(string walletId, bool includePaymentMethod = false);
        Task UpdateAsync(WalletTransaction transaction);
    }
}