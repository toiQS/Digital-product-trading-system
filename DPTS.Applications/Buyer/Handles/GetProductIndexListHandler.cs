using DPTS.Applications.Buyer.Dtos;
using DPTS.Applications.Buyer.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles
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
            _logger.LogInformation("Begin handling GetProductIndexListQuery");

            var result = new ProductIndexListDto();

            // Lấy danh sách sản phẩm khả dụng theo tên hoặc danh mục
            var products = await _productRepository.SearchAsync(request.Text, request.CategoryId, ProductStatus.Available);

            // Lấy danh mục kèm sản phẩm & rule điều chỉnh giá
            var categories = await _categoryRepository.GetsAsync(includeProduct: true, includeAdjustmentRule: true);

            // Gán thông tin danh mục vào kết quả trả về
            result.Categories = categories.Select(c => new CategoryIndexDto
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                CategoriesCount = c.Products.Count()
            }).ToList();

            // Tính trung bình đánh giá từng sản phẩm để phục vụ thống kê
            var averages = new List<double>();

            foreach (var product in products)
            {
                var avgRating = await _productReviewRepository.GetAverageOverallRatingAsync(product.ProductId);
                averages.Add(avgRating);
            }

            // Thống kê số lượng sản phẩm theo mức đánh giá 1-5 sao
            for (int i = 1; i <= 5; i++)
            {
                var count = averages.Count(x => x >= i && x < i + 1);
                result.Rates.Add(new RateIndexDto
                {
                    RatingOverall = i,
                    Count = count
                });
            }

            // Duyệt từng sản phẩm để xây dựng thông tin chi tiết
            foreach (var product in products)
            {
                var image = await _productImageRepository.GetPrimaryAsync(product.ProductId);
                if (image == null)
                {
                    _logger.LogError("Missing primary image for productId: {ProductId}", product.ProductId);
                    return ServiceResult<ProductIndexListDto>.Error("Không tìm thấy ảnh đại diện sản phẩm.");
                }

                var averageRating = await _productReviewRepository.GetAverageOverallRatingAsync(product.ProductId);
                var reviewCount = await _productReviewRepository.CountByProductIdAsync(product.ProductId);

                var category = categories.FirstOrDefault(x => x.CategoryId == product.CategoryId);
                if (category == null)
                {
                    _logger.LogError("Category not found for productId: {ProductId}", product.ProductId);
                    return ServiceResult<ProductIndexListDto>.Error("Không xác định được danh mục sản phẩm.");
                }

                var priceResult = await _adjustmentHandle.HandleDiscountAnđPriceForProduct(product);
                if (priceResult.Status != StatusResult.Success || priceResult.Data == null)
                {
                    _logger.LogError("Adjustment calculation failed for productId: {ProductId}", product.ProductId);
                    return ServiceResult<ProductIndexListDto>.Error("Không tính được giá sản phẩm sau điều chỉnh.");
                }

                result.ProductIndexList.Add(new ProductIndexDto
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    CategoryName = category.CategoryName,
                    ProductImage = image.ImagePath,
                    RatingOverallAverage = averageRating,
                    RatingOverallCount = reviewCount,
                    Price = priceResult.Data.FinalAmount,
                });
            }

            // Sắp xếp sản phẩm theo số lượt đánh giá giảm dần và phân trang
            result.ProductIndexList = result.ProductIndexList
                .OrderByDescending(x => x.RatingOverallCount)
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            _logger.LogInformation("GetProductIndexListQuery handled successfully");

            return ServiceResult<ProductIndexListDto>.Success(result);
        }
    }
}
