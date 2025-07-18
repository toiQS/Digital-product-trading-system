using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Products
{
    public interface IProductQuery
    {
        Task<Product?> GetByIdAsync(string productId, ProductQueryOptions? options = null);

        Task<List<Product>> GetByStoreIdAsync(string storeId, ProductStatus? status = null);

        Task<List<Product>> SearchAsync(ProductSearchCriteria criteria);

        Task<List<Product>> GetByCategoryAsync(string categoryId, ProductStatus? status = ProductStatus.Available);

        Task<bool> IsProductOwnedByStoreAsync(string productId, string storeId);

        Task<decimal?> GetAverageRatingAsync(string productId);

        Task<bool> IsProductAvailableAsync(string productId);
    }
    public class ProductQueryOptions
    {
        public bool IncludeImages { get; set; } = false;
        public bool IncludeReviews { get; set; } = false;
        public bool IncludeAdjustments { get; set; } = false;
    }
    public class ProductSearchCriteria
    {
        public string Keyword { get; set; } = string.Empty;
        public string? CategoryId { get; set; }
        public ProductStatus? Status { get; set; } = ProductStatus.Available;
    }

}
