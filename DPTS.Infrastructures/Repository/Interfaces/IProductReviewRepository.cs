using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IProductReviewRepository
    {
        Task AddAsync(ProductReview review);
        Task<int> CountByProductIdAsync(string productId);
        Task DeleteAsync(string reviewId);
        Task<double> GetAverageOverallRatingAsync(string productId);
        Task<IEnumerable<ProductReview>> GetByProductIdAsync(string productId, int skip = 0, int take = 10);
        Task<ProductReview?> GetByUserAndProductAsync(string userId, string productId);
        Task<IEnumerable<ProductReview>> GetTopLikedReviewsAsync(string productId, int take = 3);
        Task UpdateAsync(ProductReview review);
        Task<IEnumerable<ProductReview>> GetAllAsync();
        Task<ProductReview?> GetById(string reviewId);
    }
}