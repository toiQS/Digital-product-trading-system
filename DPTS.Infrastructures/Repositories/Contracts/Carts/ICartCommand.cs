using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Carts
{
    public interface ICartCommand
    {
        Task AddAsync(Cart cart);

        Task UpdateAsync(Cart cart);

        Task RemoveAsync(Cart cart);
    }
}
