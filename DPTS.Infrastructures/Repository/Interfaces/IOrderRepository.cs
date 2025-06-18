using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task DeleteAsync(string id);
        Task<Order?> GetByIdAsync(string id, bool includeBuyer = false, bool includeEscrow = false, bool includeComplaints = false, bool includeMessages = false, bool includeOrderItems = false);
        Task<IEnumerable<Order>> GetsAsync(string? buyerId = null, bool? isPaied = null, DateTime? fromDate = null, DateTime? toDate = null, bool includeBuyer = false, bool includeEscrow = false, bool includeComplaints = false, bool includeMessages = false, bool includeOrderItems = false);
        Task UpdateAsync(Order order);
    }

}