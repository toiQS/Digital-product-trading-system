using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Interfaces
{
    public interface IProductImageRepository
    {
        #region Query

        Task<List<ProductImage>> GetImagesByProductIdAsync(string productId);

        Task<ProductImage?> GetPrimaryImageAsync(string productId);

        Task<bool> ExistsAsync(string imageId);

        #endregion

        #region Crud

        Task AddAsync(ProductImage image);
        Task RemoveAsync(ProductImage image);
        Task RemoveByProductIdAsync(string productId);

        #endregion
    }
}
