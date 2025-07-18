using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Products
{
    public interface IProductCommand
    {
        Task RemoveAsync(Product product);
        Task AddAsync(Product product);
        Task UpdateAsync(Product product);

    }
}
