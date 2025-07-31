using DPTS.Applications.Buyer.Dtos.product;
using DPTS.Applications.Buyer.Queries.product;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles.product
{
    public class GetProductIndexListHandler : IRequestHandler<GetProductIndexListQuery, ServiceResult<ProductIndexListDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IAdjustmentHandle _adjustmentHandle;
        private readonly ILogger<GetProductIndexListHandler> _logger;

        public GetProductIndexListHandler(
            IProductRepository productRepository,
            IProductReviewRepository productReviewRepository,
            IProductImageRepository productImageRepository,
            ICategoryRepository categoryRepository,
            IAdjustmentHandle adjustmentHandle,
            ILogger<GetProductIndexListHandler> logger)
        {
            _productRepository = productRepository;
            _productReviewRepository = productReviewRepository;
            _productImageRepository = productImageRepository;
            _categoryRepository = categoryRepository;
            _adjustmentHandle = adjustmentHandle;
            _logger = logger;
        }

        public async Task<ServiceResult<ProductIndexListDto>> Handle(GetProductIndexListQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling get product list with condition for buyer");
            ProductIndexListDto result = new();
            IEnumerable<Product> products = (await _productRepository.SearchAsync()).Where(x => x.Status == ProductStatus.Available);
            var checkpointProducts = products.Count();
            List<Product> productResult = new List<Product>();
            
            if (request.Condition.CategoryIds.Any())
            {
                request.Condition.CategoryIds.ForEach(c =>
                {
                    List<Product> p = products.Where(x => x.CategoryId == c).ToList();
                    productResult.AddRange(p);
                });
                products = productResult;
                var checkpoint = products.Count();
            }

            

            foreach (Product product in products)
            {

                ProductImage? image = await _productImageRepository.GetPrimaryAsync(product.ProductId);
                if (image == null)
                {
                    _logger.LogError("Missing primary image for productId: {ProductId}", product.ProductId);
                    return ServiceResult<ProductIndexListDto>.Error("Không tìm thấy ảnh đại diện sản phẩm.");
                }

                double averageRating = await _productReviewRepository.GetAverageOverallRatingAsync(product.ProductId);
                int reviewCount = await _productReviewRepository.CountByProductIdAsync(product.ProductId);

                Category? category = await _categoryRepository.GetByIdAsync(product.CategoryId);
                if (category == null)
                {
                    _logger.LogError("Category not found for productId: {ProductId}", product.ProductId);
                    return ServiceResult<ProductIndexListDto>.Error("Không xác định được danh mục sản phẩm.");
                }

                ServiceResult<Dtos.shared.MathResultDto> priceResult = await _adjustmentHandle.HandleDiscountAndPriceForProduct(product);
                if (priceResult.Status != StatusResult.Success || priceResult.Data == null)
                {
                    _logger.LogError("Adjustment calculation failed for productId: {ProductId}", product.ProductId);
                    return ServiceResult<ProductIndexListDto>.Error("Không tính được giá sản phẩm sau điều chỉnh.");
                }
                ProductIndexDto index = new()
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    CategoryName = category.CategoryName,
                    ProductImage = image.ImagePath,
                    RatingOverallAverage = averageRating,
                    RatingOverallCount = reviewCount,
                    Price = priceResult.Data.FinalAmount,
                };
                result.ProductIndexList.Add(index);
            }
            
            if (!string.IsNullOrEmpty(request.Condition.Text))
            {
                result.ProductIndexList = result.ProductIndexList.Where(x => x.ProductId.ToLower().Contains(request.Condition.Text.ToLower()) || x.ProductName.ToLower().Contains(request.Condition.Text.ToLower())).ToList();
            }
            if (request.Condition.RatingOverall > 0)
            {
                result.ProductIndexList = result.ProductIndexList.Where(x => x.RatingOverallAverage >= request.Condition.RatingOverall).ToList();
            }
            result.TotalCount = result.ProductIndexList.Count;
            if (request.PageNumber > 0 && request.PageSize > 0)
            {
                result.ProductIndexList = result.ProductIndexList.Skip((request.PageNumber - 1) * request.PageSize).Take(request.PageSize).ToList();
            }
            
            return ServiceResult<ProductIndexListDto>.Success(result);
        }
    }
}
