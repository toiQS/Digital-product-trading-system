using DPTS.Applications.Dtos;
using DPTS.Applications.Shareds;

namespace DPTS.Applications.Interfaces
{
    public interface IProductService
    {
        
        #region Buyer
        Task<ServiceResult<IEnumerable<ProductIndexDto>>> GetProductsAsync(int pageNumber = 1, int pageSize = 10);
        Task<ServiceResult<IEnumerable<ProductIndexDto>>> GetProductsBestSaleAsync(int pageNumber = 1, int pageSize = 10);
        #endregion
        #region  Seller
        Task<ServiceResult<IEnumerable<ProductIndexDto>>> GetProductsBySellerIdAsync(string sellerId, int pageNumber = 1, int pageSize = 10);
        Task<ServiceResult<IEnumerable<ProductIndexDto>>> GetProductsBestSaleWithSellerIdAsync(string sellerId, int pageNumber = 1, int pageSize = 10);
        #endregion
        Task<ServiceResult<IEnumerable<ProductIndexDto>>> GetProductsWithManyOptions(string text, string categoryId, int rating, int pageNumber = 1, int pageSize = 10);
        Task<ServiceResult<ProductDetailDto>> GetProductByIdAsync(string productId);
    }
}
