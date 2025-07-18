using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.ProductImages
{
    public interface IProductImageQuery
    {
        Task<List<ProductImage>> GetImagesByProductIdAsync(string productId);
        Task<ProductImage?> GetPrimaryImageAsync(string productId);
        Task<bool> ExistsAsync(string imageId);
    }

}
