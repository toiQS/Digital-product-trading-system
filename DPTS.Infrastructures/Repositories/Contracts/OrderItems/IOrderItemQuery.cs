using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.OrderItems
{
    public interface IOrderItemQuery
    {
        Task<List<OrderItem>> GetByOrderIdAsync(string orderId);

        Task<OrderItem?> GetByIdAsync(string itemId);

        Task<List<OrderItem>> GetByProductIdAsync(string productId);

        Task<List<OrderItem>> GetByBuyerIdAsync(string buyerId, int limit = 100);

        Task<decimal> GetTotalRevenueByProductAsync(string productId);

        Task<bool> IsProductInOrderAsync(string orderId, string productId);
    }
}
