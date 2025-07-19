using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.ProductReviews
{
    public interface IProductReviewQuery
    {
        Task<double> GetAverageRatingByProductIdAsync(string productId, CancellationToken cancellationToken);
        Task<IEnumerable<ProductReview>> GetsByProductIdAsync(string productId, CancellationToken cancellationToken);
        Task<int> GetCountRatingByProductIdAsync(string productId, CancellationToken cancellationToken);
        Task<IEnumerable<ProductReview>> GetPositiveFeedbacksAsync(int take, CancellationToken cancellationToken);
    }

}
