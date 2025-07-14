using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Interfaces
{
    public interface IEscrowRepository
    {
        #region Query

        Task<Escrow?> GetByIdAsync(string escrowId);

        Task<Escrow?> GetByOrderIdAsync(string orderId);

        Task<List<Escrow>> GetByStoreIdAsync(string storeId);

        Task<List<Escrow>> GetAvailableToReleaseAsync(DateTime asOf);

        Task<List<Escrow>> GetExpiredPendingAsync(DateTime asOf);

        Task<decimal> GetTotalHoldAmountAsync(string storeId);

        #endregion

        #region Crud

        Task AddAsync(Escrow escrow);
        Task UpdateAsync(Escrow escrow);
        Task RemoveAsync(Escrow escrow);

        #endregion
    }
}
