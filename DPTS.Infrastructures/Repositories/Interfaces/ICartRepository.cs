using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Interfaces
{
    public interface ICartRepository
    {
        #region Query

        Task<Cart?> GetByBuyerIdAsync(string buyerId, bool includeItems = false);
        Task<Cart?> GetByIdAsync(string cartId, bool includeItems = false);
        Task<bool> ExistsAsync(string buyerId);
        Task<int> CountItemsAsync(string buyerId);

        // Truy xuất cart có chứa sản phẩm cụ thể (hữu ích khi xóa sản phẩm)
        Task<Cart?> FindCartContainingProductAsync(string productId, string buyerId);

        // Dùng cho thống kê dashboard (số lượng cart chưa thanh toán)
        Task<List<Cart>> GetCartsPendingCheckoutAsync(DateTime asOf);

        #endregion

        #region Crud

        Task AddAsync(Cart cart);
        Task UpdateAsync(Cart cart);
        Task RemoveAsync(Cart cart);

        #endregion
    }
}
