using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Escrows
{
    public interface IEscrowQuery
    {
        Task<Escrow?> GetByIdAsync(string escrowId);
        Task<Escrow?> GetByOrderIdAsync(string orderId);
        Task<List<Escrow>> GetByStoreIdAsync(string storeId);
        Task<List<Escrow>> GetAvailableToReleaseAsync(DateTime asOf);
        Task<List<Escrow>> GetExpiredPendingAsync(DateTime asOf);
        Task<decimal> GetTotalHoldAmountAsync(string storeId);
    }
}
