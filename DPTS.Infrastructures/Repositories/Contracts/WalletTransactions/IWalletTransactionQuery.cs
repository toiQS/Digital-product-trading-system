using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.WalletTransactions
{
    public interface IWalletTransactionQuery
    {
        Task<WalletTransaction?> GetByIdAsync(string transactionId);
        Task<List<WalletTransaction>> GetByWalletIdAsync(string walletId);
        Task<List<WalletTransaction>> GetByStatusAsync(WalletTransactionStatus status);
        Task<List<WalletTransaction>> GetTransactionsByTypeAsync(string walletId, TransactionType type);
    }
}
