using DPTS.Applications.Buyer.Dtos;
using DPTS.Applications.Buyer.Dtos.product;
using DPTS.Applications.Buyer.Queries.product;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles.product
{
    public class GetProductDetailHandler : IRequestHandler<GetProductDetailQuery, ServiceResult<ProductDetailDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IUserRepository _userRepository;
        private readonly IAdjustmentHandle _adjustmentHandle;
        private readonly ILogger<GetProductDetailHandler> _logger;

        public GetProductDetailHandler(
            IProductRepository productRepository,
            IProductReviewRepository productReviewRepository,
            IProductImageRepository productImageRepository,
            ICategoryRepository categoryRepository,
            IStoreRepository storeRepository,
            IUserProfileRepository userProfileRepository,
            IUserRepository userRepository,
            IAdjustmentHandle adjustmentHandle,
            ILogger<GetProductDetailHandler> logger)
        {
            _productRepository = productRepository;
            _productReviewRepository = productReviewRepository;
            _productImageRepository = productImageRepository;
            _categoryRepository = categoryRepository;
            _storeRepository = storeRepository;
            _userProfileRepository = userProfileRepository;
            _userRepository = userRepository;
            _adjustmentHandle = adjustmentHandle;
            _logger = logger;
        }

        public async Task<ServiceResult<ProductDetailDto>> Handle(GetProductDetailQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting product detail for productId: {ProductId}", request.ProjectId);

            // Lấy thông tin sản phẩm
            Product? product = await _productRepository.GetByIdAsync(request.ProjectId);
            if (product == null)
            {
                _logger.LogError("Không tìm thấy sản phẩm với ID: {ProductId}", request.ProjectId);
                return ServiceResult<ProductDetailDto>.Error("Không tìm thấy sản phẩm.");
            }

            // Lấy thông tin cửa hàng
            Store? store = await _storeRepository.GetByIdAsync(product.StoreId);
            if (store == null)
            {
                _logger.LogError("Không tìm thấy cửa hàng với ID: {StoreId}", product.StoreId);
                return ServiceResult<ProductDetailDto>.Error("Không tìm thấy thông tin cửa hàng.");
            }
            var seller = await _userRepository.GetByIdAsync(store.UserId);
            if (seller == null)
            {
                _logger.LogError("Không tìm thấy người bán với ID: {SellerId}", store.UserId);
                return ServiceResult<ProductDetailDto>.Error("Không tìm thấy thông tin người bán.");
            }
            // Tính toán giảm giá và giá cuối cùng
            var discountAndFinalPriceProduct = await _adjustmentHandle.HandleDiscountAndPriceForProduct(product);
            if (discountAndFinalPriceProduct.Status == StatusResult.Errored)
            {
                _logger.LogError("Lỗi khi tính giá sản phẩm.");
                return ServiceResult<ProductDetailDto>.Error("Không thể tính giá sản phẩm.");
            }

            // Lấy danh sách ảnh sản phẩm
            var images = await _productImageRepository.GetByProductIdAsync(product.ProductId);

            // Lấy danh sách đánh giá sản phẩm
            var productReviews = await _productReviewRepository.GetByProductIdAsync(product.ProductId, 0, 0);

            // Tính thống kê đánh giá theo số sao
            int vote1 = productReviews.Count(x => x.RatingOverall is >= 1 and < 2);
            int vote2 = productReviews.Count(x => x.RatingOverall is >= 2 and < 3);
            int vote3 = productReviews.Count(x => x.RatingOverall is >= 3 and < 4);
            int vote4 = productReviews.Count(x => x.RatingOverall is >= 4 and < 5);
            int vote5 = productReviews.Count(x => x.RatingOverall >= 5);

            // Khởi tạo DTO kết quả
            ProductDetailDto result = new()
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description ?? "Error",
                StoreName = store.StoreName,
                Discount = discountAndFinalPriceProduct.Data.Value,
                Price = discountAndFinalPriceProduct.Data.FinalAmount,
                OriginalPrice = product.OriginalPrice,
                ProductImage = images.Select(x => x.ImagePath).ToList(),
                CountReviews = productReviews.Count(),
                StoreImage = store.StoreImage,
                RatingOverall = await _productReviewRepository.GetAverageOverallRatingAsync(product.ProductId),
                SummaryFeature = product.SummaryFeature,
                Vote1 = vote1,
                Vote2 = vote2,
                Vote3 = vote3,
                Vote4 = vote4,
                Vote5 = vote5,
                StoreId = store.StoreId,
                SellerId = seller.UserId,
            };

            // Xử lý từng đánh giá người dùng
            foreach (var item in productReviews)
            {
                var user = await _userRepository.GetByIdAsync(item.UserId);
                if (user == null)
                {
                    _logger.LogError("Không tìm thấy người dùng với ID: {UserId}", item.UserId);
                    return ServiceResult<ProductDetailDto>.Error("Không tìm thấy người dùng đánh giá.");
                }

                var profile = await _userProfileRepository.GetByUserIdAsync(item.UserId);
                if (profile == null)
                {
                    _logger.LogError("Không tìm thấy hồ sơ người dùng với ID: {UserId}", item.UserId);
                    return ServiceResult<ProductDetailDto>.Error("Không tìm thấy hồ sơ người dùng.");
                }

                result.ProductReviews.Add(new ProductReviewIndexDto()
                {
                    FullName = profile.FullName ?? "Error",
                    Comment = item.Comment,
                    UserName = user.Username,
                    CreatedAt = item.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                    ImageUser = profile.ImageUrl ?? "Error",
                    Likes = item.Likes,
                    RatingOverall = item.RatingValue,
                    ReviewId = item.ReviewId,
                    UserId = user.UserId,
                });
            }

            // Lấy danh sách sản phẩm gợi ý
            var productSuggests = await ProductSuggestAsync(product);
            result.ProductSuggest = productSuggests.Data.ToList();

            return ServiceResult<ProductDetailDto>.Success(result);
        }

        private async Task<ServiceResult<IEnumerable<ProductIndexDto>>> ProductSuggestAsync(Product productInput)
        {
            _logger.LogInformation("Lấy danh sách sản phẩm gợi ý theo danh mục của sản phẩm {ProductId}", productInput.ProductId);

            var result = new List<ProductIndexDto>();
            var category = await _categoryRepository.GetByIdAsync(productInput.CategoryId, includeProduct: true);
            if (category == null)
            {
                _logger.LogError("Không tìm thấy danh mục sản phẩm với ID: {CategoryId}", productInput.CategoryId);
                return ServiceResult<IEnumerable<ProductIndexDto>>.Error("Không tìm thấy danh mục sản phẩm.");
            }

            foreach (var product in category.Products)
            {
                var image = await _productImageRepository.GetPrimaryAsync(product.ProductId);
                if (image == null)
                {
                    _logger.LogError("Không tìm thấy ảnh chính của sản phẩm {ProductId}", product.ProductId);
                    return ServiceResult<IEnumerable<ProductIndexDto>>.Error("Không tìm thấy ảnh sản phẩm.");
                }

                var averageRating = await _productReviewRepository.GetAverageOverallRatingAsync(product.ProductId);
                var reviewCount = await _productReviewRepository.CountByProductIdAsync(product.ProductId);
                var discountAndFinalPriceProduct = await _adjustmentHandle.HandleDiscountAndPriceForProduct(product);
                if (discountAndFinalPriceProduct.Status == StatusResult.Errored)
                {
                    _logger.LogError("Không thể tính toán giá cho sản phẩm gợi ý {ProductId}", product.ProductId);
                    return ServiceResult<IEnumerable<ProductIndexDto>>.Error("Không thể tính giá sản phẩm gợi ý.");
                }

                result.Add(new ProductIndexDto()
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    CategoryName = category.CategoryName,
                    ProductImage = image.ImagePath,
                    RatingOverallAverage = averageRating,
                    RatingOverallCount = reviewCount,
                    Price = discountAndFinalPriceProduct.Data.FinalAmount,
                });
            }

            return ServiceResult<IEnumerable<ProductIndexDto>>.Success(
                result.OrderByDescending(x => x.RatingOverallAverage)
                      .ThenByDescending(x => x.RatingOverallCount)
                      .Take(10));
        }
    }
}
