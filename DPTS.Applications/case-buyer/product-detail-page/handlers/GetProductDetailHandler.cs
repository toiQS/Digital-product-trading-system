using DPTS.Applications.case_buyer.product_detail_page.dtos;
using DPTS.Applications.case_buyer.product_detail_page.models;
using DPTS.Applications.shareds;
using DPTS.Applications.shareds.maths;
using DPTS.Domains;
using DPTS.Infrastructures.Repositories.Contracts.AdjustmentRules;
using DPTS.Infrastructures.Repositories.Contracts.Categories;
using DPTS.Infrastructures.Repositories.Contracts.ProductAdjustments;
using DPTS.Infrastructures.Repositories.Contracts.ProductImages;
using DPTS.Infrastructures.Repositories.Contracts.ProductReviews;
using DPTS.Infrastructures.Repositories.Contracts.Products;
using DPTS.Infrastructures.Repositories.Contracts.Stores;
using DPTS.Infrastructures.Repositories.Contracts.UserProfiles;
using MediatR;
using Microsoft.AspNetCore.Http.Timeouts;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.case_buyer.product_detail_page.handlers
{
    public class GetProductDetailHandler : IRequestHandler<GetProductDetailQuery, ServiceResult<ProductDetailDto>>
    {
        private readonly IProductImageQuery _productImageQuery;
        private readonly IProductQuery _productQuery;
        private readonly IProductReviewQuery _productReviewQuery;
        private readonly ICategoryQuery _categoryQuery;
        private readonly ILogger<GetProductDetailHandler> _logger;
        private readonly IProductAdjustmentQuery _productAdjustmentQuery;
        private readonly IAdjustmentRuleQuery _adjustmentRuleQuery;
        private readonly IStoreQuery _storeQuery;
        private readonly IUserProfileQuery _userProfileQuery;

        public GetProductDetailHandler(IProductImageQuery productImageQuery,
                                       IProductQuery productQuery,
                                       IProductReviewQuery productReviewQuery,
                                       ICategoryQuery categoryQuery,
                                       ILogger<GetProductDetailHandler> logger,
                                       IProductAdjustmentQuery productAdjustmentQuery,
                                       IAdjustmentRuleQuery adjustmentRuleQuery,
                                       IStoreQuery storeQuery,
                                       IUserProfileQuery userProfileQuery)
        {
            _productImageQuery = productImageQuery;
            _productQuery = productQuery;
            _productReviewQuery = productReviewQuery;
            _categoryQuery = categoryQuery;
            _logger = logger;
            _productAdjustmentQuery = productAdjustmentQuery;
            _adjustmentRuleQuery = adjustmentRuleQuery;
            _storeQuery = storeQuery;
            _userProfileQuery = userProfileQuery;
        }

        public async Task<ServiceResult<ProductDetailDto>> Handle(GetProductDetailQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetProductDetailQuery for ProductId: {ProductId}", request.ProductId);
            var result = new ProductDetailDto();
            Product? product = await _productQuery.GetByIdAsync(request.ProductId, cancellationToken);
            if (product == null)
            {
                _logger.LogWarning("Product not found for ProductId: {ProductId}", request.ProductId);
                return ServiceResult<ProductDetailDto>.Error($"Không tìm thấy sản phẩm với mã số {request.ProductId}.");
            }
            Store? store = await _storeQuery.GetByIdAsync(product.StoreId, cancellationToken);
            if (store == null)
            {
                _logger.LogWarning("Store not found for StoreId: {StoreId}", product.StoreId);
                return ServiceResult<ProductDetailDto>.Error($"Không tìm thấy cửa hàng với mã số {product.StoreId}.");
            }
            IEnumerable<ProductImage> productImages = await _productImageQuery.GetByProductIdAsync(request.ProductId, cancellationToken);
            result.ProductImages = productImages.Select(pi => new ProductImageIndexDto
            {
                ImageId = pi.ImageId,
                ImageUrl = pi.ImagePath
            }).ToList();
           var category = await _categoryQuery.GetByIdAsync(product.CategoryId, cancellationToken);
            if (category == null)
            {
                _logger.LogWarning("Category not found for CategoryId: {CategoryId}", product.CategoryId);
                return ServiceResult<ProductDetailDto>.Error($"Không tìm thấy danh mục với mã số {product.CategoryId}.");
            }
            var productReviews = await _productReviewQuery.GetsByProductIdAsync(request.ProductId, cancellationToken);
            if (productReviews == null || !productReviews.Any())
            {
                result.CustomersFeelings = new CustomersFeelings
                {
                    Rate1AndOver = 0,
                    Rate2AndOver = 0,
                    Rate3AndOver = 0,
                    Rate4AndOver = 0,
                    Rate5AndOver = 0,
                    PositiveReviews = new List<ProductReviewIndexDto>()
                };
            }
            else
            {
                var positiveReviews = productReviews.Where(pr => pr.RatingOverall >= 4).Take(5);
                result.CustomersFeelings = new CustomersFeelings
                {
                    Rate1AndOver = productReviews.Count(pr => pr.RatingOverall >= 1 && pr.RatingOverall < 2),
                    Rate2AndOver = productReviews.Count(pr => pr.RatingOverall >= 2 && pr.RatingOverall < 3),
                    Rate3AndOver = productReviews.Count(pr => pr.RatingOverall >= 3 && pr.RatingOverall < 4),
                    Rate4AndOver = productReviews.Count(pr => pr.RatingOverall >= 4 && pr.RatingOverall < 5),
                    Rate5AndOver = productReviews.Count(pr => pr.RatingOverall == 5),

                    
                };
                foreach(var review in positiveReviews)
                {
                    var customer = await _userProfileQuery.GetByIdAsync(review.UserId, cancellationToken);
                    if (customer == null)
                    {
                        _logger.LogError("Customer not found for UserId: {UserId}", review.UserId);
                        return ServiceResult<ProductDetailDto>.Error($"Không tìm thấy khách hàng với mã số {review.UserId}.");
                    }
                    var reviewDto = new ProductReviewIndexDto
                    {
                        ReviewId = review.ReviewId,
                        CustomerId = customer.UserId,
                        CustomerName = customer.FullName ?? "Error",
                        CustomerImage = customer.ImageUrl ??"Error" ,
                        ReviewContent = review.Comment,
                        Rating = review.RatingOverall,
                        CreatedAt = review.CreatedAt
                    };
                    result.CustomersFeelings.PositiveReviews.Add(reviewDto);
                }
                
            }
            result.ProductOverview = new ProductOverviewDto
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                ProductDescription = product.Description??"",
                CategoryName = category.CategoryName,
                StoreName = store.StoreName,
                StoreImage = store.StoreImage,
                SummaryFeature = product.SummaryFeature,
                OriginalPrice = product.OriginalPrice,
            };
            var productAdjusments = await _productAdjustmentQuery.GetByProductIdAsync(request.ProductId, cancellationToken);
            if(productAdjusments.Count() > 1)
            {
                _logger.LogWarning("Multiple product adjustments found for ProductId: {ProductId}", request.ProductId);
            }
            if(productAdjusments.Count() > 0)
            {
                var productAdjustment = productAdjusments.First();
                var adjustmentRule = await _adjustmentRuleQuery.GetByIdAsync(productAdjustment.RuleId, cancellationToken);
                if(adjustmentRule == null)
                {
                    _logger.LogWarning("Adjustment rule not found for RuleId: {RuleId}", productAdjustment.RuleId);
                    return ServiceResult<ProductDetailDto>.Error($"Không tìm thấy quy tắc điều chỉnh với mã số {productAdjustment.RuleId}.");
                }
                var mathPriceProduct = MathForProduct.CalculatePrice(product, adjustmentRule);
                result.ProductOverview.Price = mathPriceProduct.FinalPrice;
                result.ProductOverview.Discount = mathPriceProduct.DiscountValue;

            }
            else
            {
                result.ProductOverview.Price = product.OriginalPrice;
                result.ProductOverview.Discount = 0;
            }

            result.ProductOverview.OverallRating = await _productReviewQuery.GetAverageRatingByProductIdAsync(request.ProductId, cancellationToken);
            result.ProductOverview.TotalReviews = await _productReviewQuery.GetCountRatingByProductIdAsync(request.ProductId,cancellationToken);
            return ServiceResult<ProductDetailDto>.Success(result);
        }
    }
}
