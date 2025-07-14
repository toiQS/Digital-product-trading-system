using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Interfaces
{
    public interface IProductRepository
    {
        #region Query

        Task<Product?> GetByIdAsync(string productId, bool includeDetails = false);

        Task<List<Product>> GetByStoreIdAsync(string storeId, ProductStatus? status = null);

        Task<List<Product>> SearchAsync(string keyword, string? categoryId = null, ProductStatus? status = ProductStatus.Available);

        Task<List<Product>> GetByCategoryAsync(string categoryId, ProductStatus? status = ProductStatus.Available);

        Task<bool> IsProductOwnedByStoreAsync(string productId, string storeId);

        Task<decimal?> GetAverageRatingAsync(string productId);

        Task<bool> IsProductAvailableAsync(string productId);

        #endregion

        #region Crud

        Task AddAsync(Product product);
        Task UpdateAsync(Product product);
        Task RemoveAsync(Product product);

        #endregion
    }
}
