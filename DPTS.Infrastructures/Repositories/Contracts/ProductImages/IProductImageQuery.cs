using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.ProductImages
{
    public interface IProductImageQuery
    {
        Task<IEnumerable<ProductImage>> GetByProductIdAsync(string productId, CancellationToken cancellationToken);
        Task<ProductImage?> GetPrimaryImageByProductIdAsync(string productId, CancellationToken cancellationToken);
    }

}
