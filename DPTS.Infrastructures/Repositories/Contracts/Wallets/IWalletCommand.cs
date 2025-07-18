using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Wallets
{
    public interface IWalletCommand
    {
        Task CreateAsync(Wallet wallet);
        Task UpdateBalanceAsync(string walletId, decimal newBalance);
        Task UpdateLockedBalanceAsync(string walletId, decimal newLockedBalance);
    }
}
