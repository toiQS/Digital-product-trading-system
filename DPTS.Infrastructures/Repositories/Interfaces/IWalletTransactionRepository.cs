using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Interfaces
{
    public interface IWalletTransactionRepository
    {
        #region Query

        Task<WalletTransaction?> GetByIdAsync(string transactionId);

        Task<List<WalletTransaction>> GetByWalletIdAsync(string walletId);

        Task<List<WalletTransaction>> GetByStatusAsync(WalletTransactionStatus status);

        Task<decimal> GetWalletBalanceAsync(string walletId);

        Task<List<WalletTransaction>> GetTransactionsByTypeAsync(string walletId, TransactionType type);

        #endregion

        #region Command

        Task AddAsync(WalletTransaction transaction);

        Task UpdateStatusAsync(string transactionId, WalletTransactionStatus newStatus);

        #endregion

        #region Reporting / Aggregation

        Task<decimal> GetTotalAmountByTypeAsync(string walletId, TransactionType type);

        Task<int> CountTransactionsAsync(string walletId, TransactionType? type = null, WalletTransactionStatus? status = null);

        #endregion
    }
}
