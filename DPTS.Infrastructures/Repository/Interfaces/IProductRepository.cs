using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);
        Task DeleteAsync(string id);
        Task<Product?> GetByIdAsync(string id, bool includeCategory = false, bool includeImages = false, bool includeReviews = false);
        Task<IEnumerable<Product>> GetsAsync(string? sellerId = null,string? text = null, string? categoryId = null, ProductStatus? status = null, decimal? minPrice = null, decimal? maxPrice = null, string? keyword = null, bool includeCategory = false, bool includeImages = false, bool includeReviews = false);
        Task UpdateAsync(Product product);
    }
}