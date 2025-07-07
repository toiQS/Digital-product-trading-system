using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IOrderItemRepository
    {
        Task AddAsync(OrderItem item);
        Task AddManyAsync(IEnumerable<OrderItem> items);
        Task DeleteAsync(string orderItemId);
        Task<OrderItem?> GetByIdAsync(string orderItemId);
        Task<IEnumerable<OrderItem>> GetByOrderIdAsync(string orderId);
        Task UpdateAsync(OrderItem item);
    }
}