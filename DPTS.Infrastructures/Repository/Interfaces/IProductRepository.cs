using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);
        Task DeleteAsync(string id);
        Task<Product?> GetByIdAsync(string id, bool includeCategory = false, bool includeImages = false, bool includeReviews = false);
        Task<IEnumerable<Product>> GetsAsync(string? sellerId = null, string? categoryId = null, ProductStatus? status = null, double? minPrice = null, double? maxPrice = null, string? keyword = null, bool includeCategory = false, bool includeImages = false, bool includeReviews = false);
        Task UpdateAsync(Product product);
    }
}