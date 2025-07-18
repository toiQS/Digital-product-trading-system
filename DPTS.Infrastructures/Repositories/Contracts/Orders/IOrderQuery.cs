using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Orders
{
    public interface IOrderQuery
    {
        Task<Order?> GetByIdAsync(string orderId);

        Task<List<Order>> GetByBuyerIdAsync(string buyerId, int? limit = 50);

        Task<List<Order>> GetByStatusAsync(OrderStatus status);

        Task<List<Order>> GetPendingOrdersAsync(DateTime asOf);

        Task<List<Order>> GetOrderHistoryAsync(string userIdOrStoreId, bool isStore, int limit = 100);

        Task<bool> IsOrderBelongsToUserAsync(string orderId, string userId);

        Task<bool> IsOrderPaidAsync(string orderId);

        Task<decimal> GetTotalOrderAmountByBuyerAsync(string buyerId);
    }
}
