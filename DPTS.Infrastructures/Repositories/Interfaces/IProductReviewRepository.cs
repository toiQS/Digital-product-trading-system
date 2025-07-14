using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Interfaces
{
    public interface IProductReviewRepository
    {
        #region Query

        Task<List<ProductReview>> GetByProductIdAsync(string productId);

        Task<double> GetAverageRatingAsync(string productId);

        Task<bool> HasUserReviewedAsync(string productId, string userId);

        Task<ProductReview?> GetReviewByIdAsync(string reviewId);

        #endregion

        #region Crud

        Task AddAsync(ProductReview review);
        Task RemoveAsync(ProductReview review);
        Task UpdateAsync(ProductReview review);

        #endregion

        #region Actions

        Task LikeReviewAsync(string reviewId);
        Task<List<ProductReview>> GetMostHelpfulReviewsAsync(string productId, int take = 5);

        #endregion
    }
}
