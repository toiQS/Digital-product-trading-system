using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Interfaces
{
    public interface ICartItemRepository
    {
        #region Query

        Task<CartItem?> GetByIdAsync(string cartItemId);

        Task<List<CartItem>> GetByCartIdAsync(string cartId);

        Task<CartItem?> GetByCartAndProductAsync(string cartId, string productId);

        Task<bool> ExistsAsync(string cartId, string productId);

        Task<decimal> GetTotalPriceByCartIdAsync(string cartId);

        #endregion

        #region Crud

        Task AddAsync(CartItem item);
        Task UpdateAsync(CartItem item);
        Task RemoveAsync(CartItem item);

        Task RemoveByCartIdAsync(string cartId);
        Task RemoveByProductIdAsync(string productId); // Dùng khi admin/seller xóa sản phẩm

        #endregion
    }
}
