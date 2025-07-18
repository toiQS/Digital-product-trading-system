using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Wallets
{
    public interface IWalletQuery
    {
        Task<Wallet?> GetByUserIdAsync(string userId);
        Task<Wallet?> GetByIdAsync(string walletId);
        Task<decimal> GetBalanceAsync(string walletId);
        Task<decimal> GetLockedBalanceAsync(string walletId);
    }
}
