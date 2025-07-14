using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Interfaces
{
    public interface IProductReviewImageRepository
    {
        #region Query

        // Lấy toàn bộ ảnh của 1 review
        Task<List<ProductReviewImage>> GetByReviewIdAsync(string reviewId);

        // Lấy ảnh chính hoặc ảnh đầu tiên của 1 review
        Task<ProductReviewImage?> GetFirstImageAsync(string reviewId);

        // Lấy toàn bộ ảnh của nhiều review (dùng trong batch load)
        Task<Dictionary<string, List<ProductReviewImage>>> GetByReviewIdsAsync(List<string> reviewIds);

        #endregion

        #region Crud

        Task AddAsync(ProductReviewImage image);
        Task AddManyAsync(IEnumerable<ProductReviewImage> images);

        Task RemoveAsync(ProductReviewImage image);
        Task RemoveByIdAsync(string id);

        #endregion
    }
}
