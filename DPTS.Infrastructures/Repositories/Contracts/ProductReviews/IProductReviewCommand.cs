using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.ProductReviews
{
    public interface IProductReviewCommand
    {
        Task<bool> LikeReviewAsync(string reviewId, string userId);
        Task<bool> UnlikeReviewAsync(string reviewId, string userId);
        Task<bool> HasUserLikedReviewAsync(string reviewId, string userId);
        Task<bool> AddReviewAsync(ProductReview review);
        Task<bool> UpdateReviewAsync(ProductReview review);
        Task<bool> DeleteReviewAsync(string reviewId, string userId);

    }
}
