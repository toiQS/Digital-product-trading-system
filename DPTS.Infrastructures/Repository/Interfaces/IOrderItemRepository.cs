using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IOrderItemRepository
    {
        Task AddAsync(OrderItem item);
        Task AddRangeAsync(IEnumerable<OrderItem> items);
        Task DeleteByOrderIdAsync(string orderId);
        Task<OrderItem?> GetByIdAsync(string orderItemId, bool includeProduct = false);
        Task<IEnumerable<OrderItem>> GetByOrderIdAsync(string orderId, bool includeProduct = false);
        Task<IEnumerable<OrderItem>> GetByProductIdAsync(string productId);
        Task<decimal> GetTotalFinalByOrderIdAsync(string orderId);
        Task<int> GetTotalQuantitySoldAsync(string productId);
    }
}