using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Interfaces
{
    public interface IWalletRepository
    {
        #region Query

        Task<Wallet?> GetByUserIdAsync(string userId);

        Task<Wallet?> GetByIdAsync(string walletId);

        Task<decimal> GetBalanceAsync(string walletId);

        Task<decimal> GetLockedBalanceAsync(string walletId);

        #endregion

        #region Command

        Task CreateAsync(Wallet wallet);

        Task<bool> UpdateBalanceAsync(string walletId, decimal newBalance);

        Task<bool> UpdateLockedBalanceAsync(string walletId, decimal newLockedBalance);

        Task<bool> LockAmountAsync(string walletId, decimal amount);

        Task<bool> UnlockAmountAsync(string walletId, decimal amount);

        Task<bool> AdjustBalanceAsync(string walletId, decimal delta);

        Task<bool> AdjustLockedBalanceAsync(string walletId, decimal delta);

        #endregion
    }
}
