using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IEscrowRepository
    {
        Task AddAsync(Escrow escrow);
        Task DeleteAsync(string id);
        Task<Escrow?> GetByIdAsync(string id);
        Task<Escrow?> GetByOrderIdAsync(string orderId);
        Task<IEnumerable<Escrow>> GetBystoreIdAsync(string storeId);
        Task<IEnumerable<Escrow>> GetsAsync(string? storeId = null, EscrowStatus? status = null, DateTime? fromDate = null, DateTime? toDate = null, bool includeOrder = false, bool includeSeller = false);
        Task UpdateAsync(Escrow escrow);
    }
}