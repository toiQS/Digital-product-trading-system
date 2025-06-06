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

        // -------------------------- Buyer --------------------------
        [HttpGet("buyer")]
        public async Task<IActionResult> GetProducts([FromQuery] PagingModel model)
        {
            var result = await _productService.GetProductsAsync(
                pageNumber: model.PageNumber > 0 ? model.PageNumber : 1,
                pageSize: model.PageSize > 0 ? model.PageSize : 10);

            return HandleResult(result);
        }

        [HttpGet("buyer/best-sale")]
        public async Task<IActionResult> GetBestSaleProducts([FromQuery] PagingModel model)
        {
            var result = await _productService.GetProductsBestSaleAsync(
                pageNumber: model.PageNumber > 0 ? model.PageNumber : 1,
                pageSize: model.PageSize > 0 ? model.PageSize : 10);

            return HandleResult(result);
        }

        // -------------------------- Seller --------------------------
        [HttpGet("seller")]
        public async Task<IActionResult> GetProductsOfSeller([FromQuery] GetProductOfSellerModel model)
        {
            if (string.IsNullOrWhiteSpace(model.SellerId))
                return BadRequest("SellerId không được để trống.");

            var result = await _productService.GetProductsBySellerIdAsync(
                model.SellerId,
                pageNumber: model.PageNumber > 0 ? model.PageNumber : 1,
                pageSize: model.PageSize > 0 ? model.PageSize : 10);

            return HandleResult(result);
        }

        [HttpGet("seller/best-sale")]
        public async Task<IActionResult> GetBestSaleProductsOfSeller([FromQuery] GetProductOfSellerModel model)
        {
            if (string.IsNullOrWhiteSpace(model.SellerId))
                return BadRequest("SellerId không được để trống.");

            var result = await _productService.GetProductsBestSaleWithSellerIdAsync(
                model.SellerId,
                pageNumber: model.PageNumber > 0 ? model.PageNumber : 1,
                pageSize: model.PageSize > 0 ? model.PageSize : 10);

            return HandleResult(result);
        }

        // -------------------------- Detail --------------------------
        [HttpGet("detail")]
        public async Task<IActionResult> GetProductDetail([FromQuery] GetDetailProductModel model)
        {
            if (string.IsNullOrWhiteSpace(model.ProductId))
                return BadRequest("ProductId không được để trống.");

            var result = await _productService.GetProductByIdAsync(model.ProductId);
            return HandleResult(result);
        }

        // -------------------------- Search/Filter --------------------------
        [HttpGet("search")]
        public async Task<IActionResult> GetProductsWithFilter([FromQuery] GetProductsWithManyOptionsModel model)
        {
            var result = await _productService.GetProductsWithManyOptions(
                model.Text,
                model.CategoryId,
                model.Rating,
                pageNumber: model.PageNumber > 0 ? model.PageNumber : 1,
                pageSize: model.PageSize > 0 ? model.PageSize : 10);

            return HandleResult(result);
        }

        // -------------------------- Shared Result Handler --------------------------
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
