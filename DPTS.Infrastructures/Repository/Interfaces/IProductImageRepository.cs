using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IProductImageRepository
    {
        Task AddAsync(ProductImage image);
        Task AddRangeAsync(IEnumerable<ProductImage> images);
        Task DeleteAsync(string imageId);
        Task DeleteByProductIdAsync(string productId);
        Task<ProductImage?> GetByIdAsync(string imageId);
        Task<IEnumerable<ProductImage>> GetByProductIdAsync(string productId);
        Task<ProductImage?> GetPrimaryAsync(string productId);
        Task SetPrimaryAsync(string productId, string imageId);
        Task UpdateAsync(ProductImage primaryImage);
    }
}