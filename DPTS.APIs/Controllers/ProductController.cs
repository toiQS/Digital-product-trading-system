using DPTS.Applications.Interfaces;
using DPTS.Applications.Shareds;
using DPTS.APIs.Models;
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

        [HttpGet]
        public async Task<IActionResult> GetProducts([FromQuery] PagingModel model)
        {
            var result = await _productService.GetProductsAsync(
                model.PageNumber > 0 ? model.PageNumber : 1,
                model.PageSize > 0 ? model.PageSize : 10);

            return HandleResult(result);
        }

        [HttpGet("seller")]
        public async Task<IActionResult> GetProductsOfSeller([FromQuery] GetProductOfSellerModel model)
        {
            if (string.IsNullOrWhiteSpace(model.SellerId))
                return BadRequest("SellerId không được để trống.");

            var result = await _productService.GetProductsBySellerIdAsync(
                model.SellerId,
                model.PageNumber > 0 ? model.PageNumber : 1,
                model.PageSize > 0 ? model.PageSize : 10);

            return HandleResult(result);
        }

        [HttpGet("bestsale")]
        public async Task<IActionResult> GetBestSellingProducts([FromQuery] PagingModel model)
        {
            var result = await _productService.GetProductsBestSaleAsync(
                model.PageNumber > 0 ? model.PageNumber : 1,
                model.PageSize > 0 ? model.PageSize : 10);

            return HandleResult(result);
        }

        [HttpGet("detail")]
        public async Task<IActionResult> GetProductDetail([FromQuery] GetDetailProductModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ProductId))
                return BadRequest("ProductId không được để trống.");

            var result = await _productService.GetProductByIdAsync(model.ProductId);
            return HandleResult(result);
        }

        [HttpGet("by-category-rating")]
        public async Task<IActionResult> GetProductsByCategoryAndRating([FromQuery] GetProductsByCategoryAndRatingModel model)
        {
            if (string.IsNullOrWhiteSpace(model.CategoryId))
                return BadRequest("CategoryId không được để trống.");

            var result = await _productService.GetProductsByCategoryIdAndRating(
                model.CategoryId,
                model.Rating,
                model.PageNumber > 0 ? model.PageNumber : 1,
                model.PageSize > 0 ? model.PageSize : 10);

            return HandleResult(result);
        }

        [HttpGet("can-be-liked")]
        public async Task<IActionResult> GetProductsCanBeLiked([FromQuery] GetProductCanBeLike model)
        {
            if (string.IsNullOrWhiteSpace(model.CategoryId))
                return BadRequest("CategoryId không được để trống.");

            var result = await _productService.CanBeLikedAsync(
                model.CategoryId,
                model.PageNumber > 0 ? model.PageNumber : 1,
                model.PageSize > 0 ? model.PageSize : 10);

            return HandleResult(result);
        }

        private IActionResult HandleResult<T>(ServiceResult<T> result)
        {
            return result.Status switch
            {
                StatusResult.Success => Ok(new { result.MessageResult, result.Data }),
                StatusResult.Warning => Ok(new { result.MessageResult, result.Data }),
                StatusResult.Failed => NotFound(result.MessageResult),
                StatusResult.Errored => StatusCode(500, result.MessageResult),
                _ => BadRequest("Trạng thái không xác định.")
            };
        }
    }
}
