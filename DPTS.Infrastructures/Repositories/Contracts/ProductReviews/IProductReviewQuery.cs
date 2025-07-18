using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.ProductReviews
{
    public interface IProductReviewQuery
    {
        Task<List<ProductReview>> GetByProductIdAsync(string productId);
        Task<ProductReview?> GetReviewByIdAsync(string reviewId);
        Task<double> GetAverageRatingAsync(string productId);
        Task<bool> HasUserReviewedAsync(string productId, string userId);
        Task<List<ProductReview>> GetMostHelpfulReviewsAsync(string productId, int take = 5);
    }

}
