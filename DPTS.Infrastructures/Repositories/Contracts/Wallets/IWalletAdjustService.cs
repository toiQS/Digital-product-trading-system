namespace DPTS.Infrastructures.Repositories.Contracts.Wallets
{
    public interface IWalletAdjustService
    {
        Task<bool> LockAmountAsync(string walletId, decimal amount);
        Task<bool> UnlockAmountAsync(string walletId, decimal amount);
        Task<bool> AdjustBalanceAsync(string walletId, decimal delta);
        Task<bool> AdjustLockedBalanceAsync(string walletId, decimal delta);
    }
}
