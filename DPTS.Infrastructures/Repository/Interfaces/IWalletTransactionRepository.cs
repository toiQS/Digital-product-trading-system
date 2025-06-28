using DPTS.Domains;
using System.Transactions;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IWalletTransactionRepository
    {
        Task AddAsync(WalletTransaction transaction);
        Task UpdateAsync(WalletTransaction transaction);
        Task DeleteAsync(string transactionId);
        Task<WalletTransaction?> GetByIdAsync(string transactionId);
        Task<IEnumerable<WalletTransaction>> GetsAsync(
            string? walletId = null,
            TransactionType? type = null,
            TransactionStatus? status = null,
            DateTime? from = null,
            DateTime? to = null,
            decimal? minAmount = null,
            decimal? maxAmount = null
        );
    }

}