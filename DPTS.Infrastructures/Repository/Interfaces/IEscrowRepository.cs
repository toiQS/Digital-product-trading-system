using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IEscrowRepository
    {
        Task AddAsync(Escrow escrow);
        Task DeleteAsync(string escrowId);
        Task<bool> ExistsAsync(string escrowId);
        Task<IEnumerable<Escrow>> GetAllAsync(EscrowStatus? status = null, string? storeId = null, bool includeOrder = false, bool includeStore = false);
        Task<Escrow?> GetByIdAsync(string escrowId, bool includeOrder = false, bool includeStore = false);
        Task<Escrow?> GetByOrderIdAsync(string orderId);
        Task<IEnumerable<Escrow>> GetExpiredAsync(DateTime? before = null);
        Task<decimal> GetTotalHeldAmountAsync(string storeId);
        Task UpdateAsync(Escrow escrow);
    }
}