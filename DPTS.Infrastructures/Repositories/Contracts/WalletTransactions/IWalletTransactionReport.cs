using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.WalletTransactions
{
    public interface IWalletTransactionReport
    {
        Task<decimal> GetWalletBalanceAsync(string walletId); // Cẩn trọng: lẽ ra nên ở WalletQuery
        Task<decimal> GetTotalAmountByTypeAsync(string walletId, TransactionType type);
        Task<int> CountTransactionsAsync(string walletId, TransactionType? type = null, WalletTransactionStatus? status = null);
    }
}
