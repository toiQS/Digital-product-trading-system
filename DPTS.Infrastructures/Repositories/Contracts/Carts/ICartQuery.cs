using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Carts
{
    public interface ICartQuery
    {
        Task<Cart?> GetByBuyerIdAsync(string buyerId, bool includeItems = false);
        Task<Cart?> GetByIdAsync(string cartId, bool includeItems = false);
        Task<bool> ExistsAsync(string buyerId);
        Task<int> CountItemsAsync(string buyerId);
        Task<Cart?> FindCartContainingProductAsync(string productId, string buyerId);
        Task<List<Cart>> GetCartsPendingCheckoutAsync(DateTime asOf);
    }
}
