using DPTS.Applications.Dtos.products;
using DPTS.Applications.Shareds;

namespace DPTS.Applications.Interfaces
{
    public interface IProductService
    {
        Task<ServiceResult<IEnumerable<ProductIndexModel>>> GetProductsAsync();
        Task<ServiceResult<IEnumerable<ProductIndexModel>>> GetProductsBySellerId(string sellerId);
        Task<ServiceResult<IEnumerable<ProductIndexModel>>> ProductBestSellerAsync();
        Task<ServiceResult<ProductDetailModel>> DetailProductAsync(string productId);
        Task<ServiceResult<IEnumerable<ProductIndexModel>>> GetProductsByCategoryIdAndRating(string categoryId, int rating);
        Task<ServiceResult<IEnumerable<ProductIndexModel>>> CanBeLiked(string categoryId);
    }
}
