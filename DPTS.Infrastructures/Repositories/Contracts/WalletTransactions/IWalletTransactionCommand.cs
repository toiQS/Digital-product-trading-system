using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.WalletTransactions
{
    public interface IWalletTransactionCommand
    {
        Task AddAsync(WalletTransaction transaction);
        Task UpdateStatusAsync(string transactionId, WalletTransactionStatus newStatus);
    }
}
