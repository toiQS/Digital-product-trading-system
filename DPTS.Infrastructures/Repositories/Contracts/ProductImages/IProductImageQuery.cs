using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.ProductImages
{
    public interface IProductImageQuery
    {
        Task<ProductImage?> GetPrimaryImageByProductIdAsync(string productId, CancellationToken cancellationToken);
    }

}
