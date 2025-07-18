using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Carts
{
    public interface ICartItemCommand
    {
        Task AddAsync(CartItem item);
        Task UpdateAsync(CartItem item);
        Task RemoveAsync(CartItem item);
        Task RemoveByCartIdAsync(string cartId);
        Task RemoveByProductIdAsync(string productId); // khi admin/seller xoá sản phẩm
    }
}
