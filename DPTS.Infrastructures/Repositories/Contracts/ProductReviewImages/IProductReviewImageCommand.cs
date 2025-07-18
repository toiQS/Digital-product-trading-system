using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.ProductReviewImages
{
    public interface IProductReviewImageCommand
    {
        Task AddAsync(ProductReviewImage image);

        Task AddManyAsync(IEnumerable<ProductReviewImage> images);

        Task RemoveAsync(ProductReviewImage image);

        Task RemoveByIdAsync(string id);
    }
}
