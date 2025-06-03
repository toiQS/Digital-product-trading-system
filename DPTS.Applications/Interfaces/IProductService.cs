using DPTS.Applications.Dtos;
using DPTS.Applications.Shareds;

namespace DPTS.Applications.Interfaces
{
    public interface IProductService
    {
        Task<ServiceResult<IEnumerable<ProductIndexDto>>> GetProductsAsync(int pageNumber = 1, int pageSize = 10);
        Task<ServiceResult<IEnumerable<ProductIndexDto>>> GetProductsBySellerIdAsync(string sellerId, int pageNumber = 1, int pageSize = 10);
        Task<ServiceResult<IEnumerable<ProductIndexDto>>> GetProductsBestSaleAsync(int pageNumber = 1, int pageSize = 10);
        Task<ServiceResult<ProductDetailDto>> GetProductByIdAsync(string productId);
        Task<ServiceResult<IEnumerable<ProductIndexDto>>> GetProductsByCategoryIdAndRating(string categoryId, int rating, int pageNumber = 1, int pageSize = 10);
        Task<ServiceResult<IEnumerable<ProductIndexDto>>> CanBeLikedAsync(string categoryId, int pageNumber = 1, int pageSize = 10);
    }
}
