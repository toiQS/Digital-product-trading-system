using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.ProductReviewImages
{
    public interface IProductReviewImageQuery
    {
        Task<List<ProductReviewImage>> GetByReviewIdAsync(string reviewId);

        Task<ProductReviewImage?> GetFirstImageAsync(string reviewId);

        Task<Dictionary<string, List<ProductReviewImage>>> GetByReviewIdsAsync(List<string> reviewIds);
    }
}
