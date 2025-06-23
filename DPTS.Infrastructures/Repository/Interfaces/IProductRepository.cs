using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);
        Task DeleteAsync(string id);
        Task<Product?> GetByIdAsync(
            string id,
            bool includeCategory = false,
            bool includeImages = false,
            bool includeReviews = false,
            bool includeStore = false,
            bool includeOrderItem = false);
        Task<IEnumerable<Product>> GetsAsync(string? storeId = null,
                                             string? text = null,
                                             string? categoryId = null,
                                             ProductStatus? status = null,
                                             decimal? minPrice = null,
                                             decimal? maxPrice = null,
                                             string? keyword = null,
                                             bool includeCategory = false,
                                             bool includeImages = false,
                                             bool includeReviews = false,
                                             bool includeStore = false,
                                             bool includeOrderItem = false);
        Task UpdateAsync(Product product);
    }
}