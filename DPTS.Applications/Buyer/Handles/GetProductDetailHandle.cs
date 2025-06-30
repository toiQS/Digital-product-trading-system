using DPTS.Applications.Buyer.Dtos;
using DPTS.Applications.Buyer.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace DPTS.Applications.Buyer.Handles;

public class GetProductDetailHandle : IRequestHandler<GetProductDetailQuery, ServiceResult<ProductDetailDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IProductReviewRepository _productReviewRepository;
    private readonly IProductImageRepository _productImageRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductAdjustmentRepository _productAdjustmentRepository;
    private readonly IAdjustmentRuleRepository _adjustmentRuleRepository;
    private readonly IStoreRepository _storeRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetProductDetailHandle> _logger;

    public GetProductDetailHandle(
        IProductRepository productRepository,
        IProductReviewRepository productReviewRepository,
        IProductImageRepository productImageRepository,
        ICategoryRepository categoryRepository,
        IProductAdjustmentRepository productAdjustmentRepository,
        IAdjustmentRuleRepository adjustmentRuleRepository,
        IStoreRepository storeRepository,
        IUserProfileRepository userProfileRepository,
        IUserRepository userRepository,
        ILogger<GetProductDetailHandle> logger)
    {
        _productRepository = productRepository;
        _productReviewRepository = productReviewRepository;
        _productImageRepository = productImageRepository;
        _categoryRepository = categoryRepository;
        _productAdjustmentRepository = productAdjustmentRepository;
        _adjustmentRuleRepository = adjustmentRuleRepository;
        _storeRepository = storeRepository;
        _userProfileRepository = userProfileRepository;
        _userRepository = userRepository;
        _logger = logger;
    }

    public async Task<ServiceResult<ProductDetailDto>> Handle(GetProductDetailQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Getting product detail for ProductId: {ProductId}", request.ProjectId);

        var product = await _productRepository.GetByIdAsync(request.ProjectId);
        if (product == null)
        {
            _logger.LogError("Product not found with id {ProductId}", request.ProjectId);
            return ServiceResult<ProductDetailDto>.Error("Không tìm thấy sản phẩm.");
        }

        var store = await _storeRepository.GetByIdAsync(product.StoreId);
        if (store == null)
        {
            _logger.LogError("Store not found with id {StoreId}", product.StoreId);
            return ServiceResult<ProductDetailDto>.Error("Không tìm thấy cửa hàng.");
        }

        // Tính giá sau điều chỉnh
        var finalPriceResult = await CalculateFinalPrice(product);
        if (finalPriceResult.Status != StatusResult.Success)
            return ServiceResult<ProductDetailDto>.Error("Không thể tính giá sản phẩm.");

        var images = await _productImageRepository.GetByProductIdAsync(product.ProductId);
        var productReviews = await _productReviewRepository.GetByProductIdAsync(product.ProductId, 0, 0);
        var ratingStats = CalculateRatingStats(productReviews);

        // Tạo đối tượng ProductDetailDto
        var result = new ProductDetailDto
        {
            ProductId = product.ProductId,
            ProductName = product.ProductName,
            Description = product.Description ?? "Error",
            StoreName = store.StoreName,
            Discount = ratingStats.TotalDiscount,
            Price = finalPriceResult.Data,
            OriginalPrice = product.OriginalPrice,
            ProductImage = images.Select(x => x.ImagePath).ToList(),
            CountReviews = productReviews.Count(),
            StoreImage = store.StoreImage,
            RatingOverall = await _productReviewRepository.GetAverageOverallRatingAsync(product.ProductId),
            SummaryFeature = product.SummaryFeature,
            Vote1 = ratingStats.Vote1,
            Vote2 = ratingStats.Vote2,
            Vote3 = ratingStats.Vote3,
            Vote4 = ratingStats.Vote4,
            Vote5 = ratingStats.Vote5
        };

        // Thêm đánh giá người dùng
        foreach (var item in productReviews)
        {
            var user = await _userRepository.GetByIdAsync(item.UserId);
            var profile = await _userProfileRepository.GetByUserIdAsync(item.UserId);

            if (user == null || profile == null)
            {
                _logger.LogError("User/Profile not found for review: {ReviewId}", item.ReviewId);
                return ServiceResult<ProductDetailDto>.Error("Không thể lấy thông tin người đánh giá.");
            }

            result.ProductReviews.Add(new ProductReviewIndexDto
            {
                FullName = profile.FullName ?? "Error",
                Comment = item.Comment,
                UserName = user.Username,
                CreatedAt = item.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss"),
                ImageUser = profile.ImageUrl ?? "Error",
                Likes = item.Likes,
                RatingOverall = item.RatingValue,
                ReviewId = item.ReviewId,
                UserId = user.UserId
            });
        }

        // Gợi ý sản phẩm cùng danh mục
        var suggestions = await GetProductSuggestions(product);
        result.ProductSuggest = suggestions;

        _logger.LogInformation("Completed getting product detail for ProductId: {ProductId}", product.ProductId);
        return ServiceResult<ProductDetailDto>.Success(result);
    }

    /// <summary>
    /// Tính giá cuối cùng của sản phẩm theo các điều chỉnh giá.
    /// </summary>
    private async Task<ServiceResult<decimal>> CalculateFinalPrice(Product product)
    {
        var rules = await _productAdjustmentRepository.GetRulesByProductIdAsync(product.ProductId);

        var now = DateTime.Now;
        var taxes = new List<AdjustmentRule>();
        var discounts = new List<AdjustmentRule>();
        var platformFees = new List<AdjustmentRule>();

        foreach (var adjustment in rules)
        {
            var rule = await _adjustmentRuleRepository.GetByIdAsync(adjustment.RuleId);
            if (rule == null || rule.Status != RuleStatus.Active || rule.From > now || rule.To < now)
                continue;

            switch (rule.Type)
            {
                case AdjustmentType.Tax:
                    taxes.Add(rule);
                    break;
                case AdjustmentType.Discount:
                    discounts.Add(rule);
                    break;
                case AdjustmentType.PlatformFee:
                    platformFees.Add(rule);
                    break;
            }
        }

        decimal originalPrice = product.OriginalPrice;
        decimal totalDiscount = discounts.Sum(x => x.Value);
        decimal discountedPrice = originalPrice * (1 - totalDiscount);
        decimal priceWithFee = discountedPrice * (1 + platformFees.Sum(x => x.Value));
        decimal finalPrice = priceWithFee * (1 + taxes.Sum(x => x.Value));

        return ServiceResult<decimal>.Success(finalPrice);
    }

    /// <summary>
    /// Tính thống kê đánh giá sản phẩm theo số sao.
    /// </summary>
    private (int Vote1, int Vote2, int Vote3, int Vote4, int Vote5, decimal TotalDiscount) CalculateRatingStats(IEnumerable<ProductReview> reviews)
    {
        return (
            Vote1: reviews.Count(x => x.RatingOverall >= 1 && x.RatingOverall < 2),
            Vote2: reviews.Count(x => x.RatingOverall >= 2 && x.RatingOverall < 3),
            Vote3: reviews.Count(x => x.RatingOverall >= 3 && x.RatingOverall < 4),
            Vote4: reviews.Count(x => x.RatingOverall >= 4 && x.RatingOverall < 5),
            Vote5: reviews.Count(x => x.RatingOverall >= 5),
            TotalDiscount: 0 // giữ đây nếu sau muốn lấy tổng giảm giá tách riêng
        );
    }

    /// <summary>
    /// Gợi ý sản phẩm cùng danh mục (tối đa 10).
    /// </summary>
    private async Task<List<ProductIndexDto>> GetProductSuggestions(Product baseProduct)
    {
        var result = new List<ProductIndexDto>();

        var category = await _categoryRepository.GetByIdAsync(baseProduct.CategoryId, includeProduct: true);
        if (category == null)
        {
            _logger.LogError("Category not found with id {CategoryId}", baseProduct.CategoryId);
            return result;
        }

        foreach (var product in category.Products.Where(p => p.ProductId != baseProduct.ProductId))
        {
            var image = await _productImageRepository.GetPrimaryAsync(product.ProductId);
            if (image == null) continue;

            var averageRating = await _productReviewRepository.GetAverageOverallRatingAsync(product.ProductId);
            var reviewCount = await _productReviewRepository.CountByProductIdAsync(product.ProductId);
            var priceResult = await CalculateFinalPrice(product);

            result.Add(new ProductIndexDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                CategoryName = category.CategoryName,
                ProductImage = image.ImagePath,
                RatingOverallAverage = averageRating,
                RatingOverallCount = reviewCount,
                Price = priceResult.Data
            });
        }

        return result
            .OrderByDescending(x => x.RatingOverallAverage)
            .ThenByDescending(x => x.RatingOverallCount)
            .Take(10)
            .ToList();
    }
}
