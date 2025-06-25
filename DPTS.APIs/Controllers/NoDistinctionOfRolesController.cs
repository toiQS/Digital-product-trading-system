using DPTS.Applications.NoDistinctionOfRoles.auths.Queries;
using DPTS.Applications.NoDistinctionOfRoles.homePages.Queries;
using DPTS.Applications.NoDistinctionOfRoles.products.Queries;
using DPTS.Applications.Shareds;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DPTS.APIs.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class NoDistinctionOfRolesController : ControllerBase
    {
        private readonly IMediator _mediator;

        public NoDistinctionOfRolesController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // Auths
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeResult(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeResult(result);
        }

        [HttpPost("2fa")]
        public async Task<IActionResult> TwoFactor([FromBody] _2FAQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeResult(result);
        }

        // HomePage
        [HttpGet("top-selling-products")]
        public async Task<IActionResult> GetTopSellingProducts()
        {
            var result = await _mediator.Send(new GetTopSellingProductsQuery());
            return StatusCodeResult(result);
        }

        [HttpGet("featured-categories")]
        public async Task<IActionResult> GetFeaturedCategories()
        {
            var result = await _mediator.Send(new GetFeaturedCategoriesQuery());
            return StatusCodeResult(result);
        }

        [HttpGet("featured-reviews")]
        public async Task<IActionResult> GetFeaturedReviews()
        {
            var result = await _mediator.Send(new GetFeaturedReviewsQuery());
            return StatusCodeResult(result);
        }

        // Product
        [HttpGet("product-detail/{productId}")]
        public async Task<IActionResult> GetProductDetail(string productId)
        {
            var result = await _mediator.Send(new GetProductDetailQuery { ProductId = productId });
            return StatusCodeResult(result);
        }

        // Helper: Đọc ServiceResult và trả về status HTTP tương ứng
        private IActionResult StatusCodeResult<T>(ServiceResult<T> result)
        {
            return result.Status switch
            {
                StatusResult.Success => Ok(result),
                StatusResult.Warning => Ok(result), // 200 nhưng thông báo cảnh báo
                StatusResult.Failed => NotFound(result),
                StatusResult.Errored => StatusCode(500, result),
                _ => BadRequest(result)
            };
        }
    }
}
