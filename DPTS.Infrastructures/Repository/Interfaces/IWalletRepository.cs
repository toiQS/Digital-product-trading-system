using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IWalletRepository
    {
        Task AddAsync(Wallet wallet);
        Task AddTransactionAsync(string walletId, WalletTransaction transaction);
        Task DeleteAsync(string walletId);
        Task<bool> ExistsByUserIdAsync(string userId);
        Task<Wallet?> GetByIdAsync(string walletId, bool includeTransactions = false);
        Task<Wallet?> GetByUserIdAsync(string userId, bool includeTransactions = false);
        Task UpdateAsync(Wallet wallet);
    }
}