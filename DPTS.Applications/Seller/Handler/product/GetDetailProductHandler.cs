using DPTS.Applications.Seller.Dtos.product;
using DPTS.Applications.Seller.Query.product;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Math.EC.Rfc7748;

namespace DPTS.Applications.Seller.Handler.product
{
    public class GetDetailProductHandler : IRequestHandler<GetProductDetailQuery, ServiceResult<ProductDetailDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly IAdjustmentHandle _adjustmentHandle;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IComplaintRepository _complaintRepository;
        private readonly ILogger<GetDetailProductHandler> _logger;

        public GetDetailProductHandler(IProductRepository productRepository,
                                       IStoreRepository storeRepository,
                                       IProductImageRepository productImageRepository,
                                       IAdjustmentHandle adjustmentHandle,
                                       ICategoryRepository categoryRepository,
                                       IUserProfileRepository userProfileRepository,
                                       IComplaintRepository complaintRepository,
                                       ILogger<GetDetailProductHandler> logger)
        {
            _productRepository = productRepository;
            _storeRepository = storeRepository;
            _productImageRepository = productImageRepository;
            _adjustmentHandle = adjustmentHandle;
            _categoryRepository = categoryRepository;
            _userProfileRepository = userProfileRepository;
            _complaintRepository = complaintRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<ProductDetailDto>> Handle(GetProductDetailQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetProductDetailQuery for ProductId: {ProductId}, UserId: {UserId}", request.ProductId, request.UserId);
            var profile = await _userProfileRepository.GetByUserIdAsync(request.UserId);
            if (profile == null)
            {
                _logger.LogWarning("User profile not found for UserId: {UserId}", request.UserId);
                return ServiceResult<ProductDetailDto>.Error("Không tìm thấy tài khoản.");
            }
            var store = await _storeRepository.GetByUserIdAsync(request.UserId);
            if (store == null)
            {
                _logger.LogWarning("Store not found for UserId: {UserId}", request.UserId);
                return ServiceResult<ProductDetailDto>.Error("Không tìm thấy cửa hàng.");
            }
            var product = await _productRepository.GetByIdAsync(request.ProductId);
            if (product == null)
            {
                _logger.LogWarning("Product not found for ProductId: {ProductId}", request.ProductId);
                return ServiceResult<ProductDetailDto>.Error("Không tìm thấy sản phẩm.");
            }
            if (product.StoreId != store.StoreId)
            {
                _logger.LogWarning("Product with ProductId: {ProductId} does not belong to StoreId: {StoreId}", request.ProductId, store.StoreId);
                return ServiceResult<ProductDetailDto>.Error("Sản phẩm không thuộc cửa hàng của bạn.");
            }
            var productImages = await _productImageRepository.GetByProductIdAsync(request.ProductId);
            if (productImages == null || !productImages.Any())
            {
                _logger.LogWarning("No images found for ProductId: {ProductId}", request.ProductId);
            }
            var discountAndFinalPriceProduct = await _adjustmentHandle.HandleDiscountAndPriceForProduct(product);
            if (discountAndFinalPriceProduct.Status == StatusResult.Errored)
            {
                _logger.LogError("Error handling discount and price for ProductId: {ProductId}", request.ProductId);
                return ServiceResult<ProductDetailDto>.Error("Không tính được  giá sau điều chỉnh của giỏ hàng");

            }
            var category = await _categoryRepository.GetByIdAsync(product.CategoryId);
            if (category == null)
            {
                _logger.LogError("Category not found for CategoryId: {CategoryId}", product.CategoryId);
                return ServiceResult<ProductDetailDto>.Error("Không tìm thấy danh mục sản phẩm.");
            }
            var totalComplaints = await _complaintRepository.CountByProductIdAsync(product.ProductId);
            var dto = new ProductDetailDto()
            {
                ProductId = product.ProductId,
                Name = product.ProductName,
                CategoryName = category.CategoryName,
                CreatedAt = DateTime.UtcNow,
                Description = product.Description ?? "Error",
                DiscountedPrice = discountAndFinalPriceProduct.Data.Amount,
                DiscountedValue = discountAndFinalPriceProduct.Data.Value,
                Images = productImages.Select(img => new ImageDto
                {
                    Id = img.ImageId,
                    ImagePath = img.ImagePath
                }).ToList(),
                OriginalPrice = product.OriginalPrice,
                Price = discountAndFinalPriceProduct.Data.FinalAmount,
                TotalComplaints = totalComplaints,
                UpdatedAt = product.UpdatedAt,
                SummaryFeature = product.SummaryFeature ?? "Error"
            };
            return ServiceResult<ProductDetailDto>.Success(dto);
        }
    }
}
