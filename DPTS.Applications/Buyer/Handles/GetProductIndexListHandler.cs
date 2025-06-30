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
        private readonly IProductAdjustmentRepository _productAdjustmentRepository;
        private readonly IAdjustmentRuleRepository _adjustmentRuleRepository;
        private readonly ILogger<GetProductIndexListHandler> _logger;

        public GetProductIndexListHandler(
            IProductRepository productRepository,
            IProductReviewRepository productReviewRepository,
            IProductImageRepository productImageRepository,
            ICategoryRepository categoryRepository,
            IProductAdjustmentRepository productAdjustmentRepository,
            IAdjustmentRuleRepository adjustmentRuleRepository,
            ILogger<GetProductIndexListHandler> logger)
        {
            _productRepository = productRepository;
            _productReviewRepository = productReviewRepository;
            _productImageRepository = productImageRepository;
            _categoryRepository = categoryRepository;
            _productAdjustmentRepository = productAdjustmentRepository;
            _adjustmentRuleRepository = adjustmentRuleRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<ProductIndexListDto>> Handle(GetProductIndexListQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Bắt đầu xử lý truy vấn danh sách sản phẩm");

            var result = new ProductIndexListDto();

            // Tìm kiếm sản phẩm khả dụng theo tên hoặc danh mục
            var products = await _productRepository.SearchAsync(request.Text, request.CategoryId, ProductStatus.Available);

            // Lấy danh sách danh mục kèm sản phẩm & rule điều chỉnh giá
            var categories = await _categoryRepository.GetsAsync(includeProduct: true, includeAdjustmentRule: true);

            // Gán thông tin danh mục vào kết quả
            result.Categories = categories.Select(c => new CategoryIndexDto()
            {
                CategoryId = c.CategoryId,
                CategoryName = c.CategoryName,
                CategoriesCount = c.Products.Count(),
            }).ToList();

            // Tính trung bình đánh giá để thống kê theo mức sao
            var averages = new List<double>();

            foreach (var product in products)
            {
                var avgRating = await _productReviewRepository.GetAverageOverallRatingAsync(product.ProductId);
                averages.Add(avgRating);
            }

            // Tổng hợp số lượng sản phẩm theo mức đánh giá 1-5 sao
            for (var i = 1; i <= 5; i++)
            {
                var count = averages.Count(x => x >= i && x < i + 1);
                result.Rates.Add(new RateIndexDto()
                {
                    RatingOverall = i,
                    Count = count
                });
            }

            // Duyệt từng sản phẩm để xây dựng thông tin trả về
            foreach (var product in products)
            {
                var image = await _productImageRepository.GetPrimaryAsync(product.ProductId);
                if (image == null)
                {
                    _logger.LogError("Không tìm thấy ảnh đại diện cho sản phẩm {ProductId}", product.ProductId);
                    return ServiceResult<ProductIndexListDto>.Error("Không tìm thấy ảnh đại diện cho sản phẩm.");
                }

                var averageRating = await _productReviewRepository.GetAverageOverallRatingAsync(product.ProductId);
                var reviewCount = await _productReviewRepository.CountByProductIdAsync(product.ProductId);

                var category = categories.FirstOrDefault(x => x.CategoryId == product.CategoryId);
                if (category == null)
                {
                    _logger.LogError("Không tìm thấy danh mục cho sản phẩm {ProductId}", product.ProductId);
                    return ServiceResult<ProductIndexListDto>.Error("Không tìm thấy danh mục cho sản phẩm.");
                }

                // Tính toán giá cuối cùng dựa trên rule điều chỉnh giá
                var finalPrice = await CalculateFinalPrice(product);

                // Thêm vào danh sách kết quả
                result.ProductIndexList.Add(new ProductIndexDto()
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    CategoryName = category.CategoryName,
                    ProductImage = image.ImagePath,
                    RatingOverallAverage = averageRating,
                    RatingOverallCount = reviewCount,
                    Price = finalPrice
                });
            }
            result.ProductIndexList.OrderByDescending(x => x.RatingOverallCount).Skip((request.PageSize - 1) * request.PageSize).ToList();

            _logger.LogInformation("Hoàn tất xử lý danh sách sản phẩm");
            return ServiceResult<ProductIndexListDto>.Success(result);
        }

        /// <summary>
        /// Tính giá cuối cùng của sản phẩm dựa trên giảm giá, phí nền tảng và thuế.
        /// </summary>
        private async Task<decimal> CalculateFinalPrice(Product product)
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

            decimal totalPlatformFee = platformFees.Sum(x => x.Value);
            decimal priceWithFee = discountedPrice * (1 + totalPlatformFee);

            decimal totalTax = taxes.Sum(x => x.Value);
            decimal finalPrice = priceWithFee * (1 + totalTax);

            return finalPrice;
        }
    }
}