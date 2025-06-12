using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IOrderItemRepository
    {
        Task AddAsync(OrderItem item);
        Task DeleteAsync(string id);
        Task<OrderItem?> GetByIdAsync(string id, bool includeProduct = false, bool includeOrder = false);
        Task<IEnumerable<OrderItem>> GetsAsync(string? orderId = null, string? productId = null, double? minAmount = null, double? maxAmount = null, int? minQuantity = null, int? maxQuantity = null, bool includeProduct = false, bool includeOrder = false);
        Task UpdateAsync(OrderItem item);
    }
}