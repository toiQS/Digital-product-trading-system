using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IProductImageRepository
    {
        Task AddAsync(ProductImage image);
        Task DeleteAsync(string id);
        Task<ProductImage?> GetByIdAsync(string id, bool includeProduct = false);
        Task<IEnumerable<ProductImage>> GetsAsync(string? search = null, bool? isPrimary = null, DateTime? from = null, DateTime? to = null, string? productId = null, bool includeProduct = false);
        Task UpdateAsync(ProductImage image);
    }
}