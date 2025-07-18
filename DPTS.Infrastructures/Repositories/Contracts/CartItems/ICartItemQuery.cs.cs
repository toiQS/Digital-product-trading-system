using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.CartItems
{
    public interface ICartItemQuery
    {
        Task<CartItem?> GetByIdAsync(string cartItemId);
        Task<List<CartItem>> GetByCartIdAsync(string cartId);
        Task<CartItem?> GetByCartAndProductAsync(string cartId, string productId);
        Task<bool> ExistsAsync(string cartId, string productId);
        Task<decimal> GetTotalPriceByCartIdAsync(string cartId);
    }
}
