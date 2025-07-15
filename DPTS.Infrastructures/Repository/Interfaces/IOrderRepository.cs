using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task DeleteAsync(string orderId);
        Task<bool> ExistsAsync(string orderId);
        Task<IEnumerable<Order>> GetByBuyerAsync(string buyerId, bool onlyPaid = false, bool includeItems = false);
        Task<Order?> GetByIdAsync(string orderId, bool includeItems = false, bool includeEscrows = false, bool includeBuyer = false);
        Task<IEnumerable<Order>> GetsAsync(bool includeBuyer = false, bool includeEscrow = false);
        Task<decimal> GetTotalSpentAsync(string buyerId);
        Task<bool> IsPaidAsync(string orderId);
        Task UpdateAsync(Order order);
    }
}