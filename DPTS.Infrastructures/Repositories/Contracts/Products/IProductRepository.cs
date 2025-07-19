using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Products
{
    public interface IProductQuery
    {
        Task<Product?> GetByIdAsync(string productId, CancellationToken cancellationToken);
    }

}
