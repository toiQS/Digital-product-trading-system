using DPTS.Applications.Interfaces;
using DPTS.Applications.Shareds;
using Microsoft.AspNetCore.Mvc;

namespace DPTS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IProductService _productService;

        public ProductController(IProductService productService)
        {
            _productService = productService;
        }

        /// <summary>
        /// Lấy toàn bộ sản phẩm.
        /// </summary>
        [HttpGet]
        public async Task<IActionResult> GetAllProducts()
        {
            var result = await _productService.GetProductsAsync();
            return APIResult.From(result);
        }

        /// <summary>
        /// Lấy sản phẩm theo người bán.
        /// </summary>
        [HttpGet("seller/{sellerId}")]
        public async Task<IActionResult> GetProductsBySeller(string sellerId)
        {
            if (string.IsNullOrWhiteSpace(sellerId))
                return BadRequest("Mã người bán không hợp lệ.");

            var result = await _productService.GetProductsBySellerId(sellerId);
            return APIResult.From(result);
        }

        /// <summary>
        /// Lấy danh sách sản phẩm bán chạy.
        /// </summary>
        [HttpGet("bestsellers")]
        public async Task<IActionResult> GetBestSellers()
        {
            var result = await _productService.ProductBestSellerAsync();
            return APIResult.From(result);
        }

        /// <summary>
        /// Lấy chi tiết sản phẩm theo mã.
        /// </summary>
        [HttpGet("{productId}")]
        public async Task<IActionResult> GetProductDetail(string productId)
        {
            if (string.IsNullOrWhiteSpace(productId))
                return BadRequest("Mã sản phẩm không hợp lệ.");

            var result = await _productService.DetailProductAsync(productId);
            return APIResult.From(result);
        }

        /// <summary>
        /// Lọc sản phẩm theo danh mục và đánh giá sao.
        /// </summary>
        [HttpGet("filter")]
        public async Task<IActionResult> FilterByCategoryAndRating(
            [FromQuery] string categoryId,
            [FromQuery] int rating)
        {
            if (string.IsNullOrWhiteSpace(categoryId) || rating < 0)
                return BadRequest("Tham số lọc không hợp lệ.");

            var result = await _productService.GetProductsByCategoryIdAndRating(categoryId, rating);
            return APIResult.From(result);
        }

        /// <summary>
        /// Gợi ý sản phẩm có thể thích trong cùng danh mục.
        /// </summary>
        [HttpGet("suggestions/{categoryId}")]
        public async Task<IActionResult> GetSuggestions(string categoryId)
        {
            if (string.IsNullOrWhiteSpace(categoryId))
                return BadRequest("Mã danh mục không hợp lệ.");

            var result = await _productService.CanBeLiked(categoryId);
            return APIResult.From(result);
        }
    }
}
