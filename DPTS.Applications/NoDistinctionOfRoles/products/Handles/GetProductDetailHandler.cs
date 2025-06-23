using DPTS.Applications.NoDistinctionOfRoles.products.Dtos;
using DPTS.Applications.NoDistinctionOfRoles.products.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using LinqKit;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Text;
using System.Threading.Tasks;

namespace DPTS.Applications.NoDistinctionOfRoles.products.Handles
{
    public class GetProductDetailHandler : IRequestHandler<GetProductDetailQuery, ServiceResult<ProductDetailDto>>
    {
        private readonly IProductRepository _productRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly ILogger<GetProductDetailHandler> _logger;

        public GetProductDetailHandler(
            IProductRepository productRepository,
            IProductImageRepository productImageRepository,
            IProductReviewRepository productReviewRepository,
            ICategoryRepository categoryRepository,
            ILogger<GetProductDetailHandler> logger)
        {
            _productRepository = productRepository;
            _productImageRepository = productImageRepository;
            _productReviewRepository = productReviewRepository;
            _categoryRepository = categoryRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<ProductDetailDto>> Handle(GetProductDetailQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching product detail for ProductId: {ProductId}", query.ProductId);

            try
            {
                // Bắt đầu lấy dữ liệu song song để tăng hiệu năng
                var productTask = _productRepository.GetByIdAsync(query.ProductId, includeStore: true);
                var imagesTask = _productImageRepository.GetsAsync(productId: query.ProductId);
                var reviewsTask = _productReviewRepository.GetsAsync(productId: query.ProductId, includeUser: true);

                await Task.WhenAll(productTask, imagesTask, reviewsTask);

                var product = await productTask;
                var productImages = await imagesTask;
                var productReviews = await reviewsTask;

                // Truy xuất thêm category nếu cần
                var categoryTask = _categoryRepository.GetByIdAsync(product.CategoryId, includeProduct: true);
                var category = await categoryTask; // Nếu không dùng thì có thể loại bỏ luôn

                // Tạo DTO kết quả
                var productDto = new ProductDetailDto
                {
                    ProductId = product.ProductId,
                    ProductName = product.ProductName,
                    StoreName = product.Store?.StoreName ?? "Error",
                    StoreImage = product.Store?.StoreImage,
                    Description = product.Description ?? "Empty",
                    Summary = FormatSummary(product.Summary),
                    Discount = product.Discount,
                    OriginalPrice = product.OriginalPrice,
                    Price = product.Price,
                    Rating = productReviews.Any() ? productReviews.Average(x => x.Rating) : 0,
                    CountReviews = productReviews.Count(),
                    ProductImage = productImages.Select(x => x.ImagePath).ToList(),
                    Vote1 = productReviews.Count(x => x.Rating == 1),
                    Vote2 = productReviews.Count(x => x.Rating == 2),
                    Vote3 = productReviews.Count(x => x.Rating == 3),
                    Vote4 = productReviews.Count(x => x.Rating == 4),
                    Vote5 = productReviews.Count(x => x.Rating == 5),
                    ProductReviewIndex = productReviews.Select(x => new ProductReviewIndexDto
                    {
                        ReviewId = x.ReviewId,
                        UserId = x.UserId,
                        UserName = x.User?.Username,
                        FullName = x.User?.FullName,
                        ImageUser = x.User?.ImageUrl,
                        Comment = x.Comment,
                        Rating = x.Rating,
                        Likes = x.Likes,
                        CreatedAt = x.CreatedAt.ToString("yyyy-MM-dd HH:mm:ss")
                    }).ToList()
                };

                return ServiceResult<ProductDetailDto>.Success(productDto);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred while fetching product detail for ProductId: {ProductId}", query.ProductId);
                return ServiceResult<ProductDetailDto>.Error("Lỗi không thể xem chi tiết sản phẩm.");
            }
        }

        /// <summary>
        /// Định dạng phần summary thành dạng bullet để hiển thị rõ ràng hơn
        /// </summary>
        private static string FormatSummary(string? summary)
        {
            if (string.IsNullOrWhiteSpace(summary))
                return "- No summary available.";

            var builder = new StringBuilder();
            foreach (var sentence in summary.Split('.', StringSplitOptions.RemoveEmptyEntries))
            {
                builder.AppendLine($"- {sentence.Trim()}.");
            }

            return builder.ToString();
        }
    }
}
