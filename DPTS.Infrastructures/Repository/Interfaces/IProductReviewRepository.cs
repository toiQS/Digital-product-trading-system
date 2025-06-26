using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IProductReviewRepository
    {
        Task AddAsync(ProductReview review);
        Task DeleteAsync(string reviewId);
        Task<ProductReview?> GetByIdAsync(string reviewId, bool includeUser = false, bool includeProduct = false);
        Task<IEnumerable<ProductReview>> GetsAsync(string? search = null, string? productId = null, string? userId = null, int? minRatingOverall = null, int? maxRatingOverall = null, DateTime? from = null, DateTime? to = null, bool includeUser = false, bool includeProduct = false);
        Task UpdateAsync(ProductReview review);
    }
}