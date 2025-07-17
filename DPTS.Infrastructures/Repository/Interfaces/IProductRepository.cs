using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IProductRepository
    {
        Task AddAsync(Product product);
        Task<int> CountByStoreAsync(string storeId, ProductStatus? status = null);
        Task DeleteAsync(string productId);
        Task<Product?> GetByIdAsync(string productId, bool includeCategory = false, bool includeStore = false, bool includeAdjustments = false);
        Task<IEnumerable<Product>> GetByListIdsAsync(List<string> productIds);
        Task<IEnumerable<Product>> GetByStoreAsync(string storeId, ProductStatus? status = null, int skip = 0, int take = 50);
        Task<IEnumerable<Product>> SearchAsync(string? keyword = null, string? categoryId = null, ProductStatus? status = ProductStatus.Available, int skip = 0, int take = 50);
        Task UpdateAsync(Product product);
        Task<Product?> GetByNameAndStoreAsync(string productName, string storeId);
    }
}