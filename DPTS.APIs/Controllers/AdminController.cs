
using DPTS.Applications.Shareds;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DPTS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AdminController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // OVERVIEW
        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview([FromQuery] string userId)
        {
            var result = await _mediator.Send(new Applications.Admin.overview.Queries.GetOverviewQuery { UserId = userId });
            return StatusCodeFromResult(result);
        }

        [HttpGet("overview/recent-activity")]
        public async Task<IActionResult> GetRecentActivity([FromQuery] string userId)
        {
            var result = await _mediator.Send(new Applications.Admin.overview.Queries.GetRecentActivityQuery { UserId = userId });
            return StatusCodeFromResult(result);
        }

        [HttpGet("overview/top-store")]
        public async Task<IActionResult> GetTopStore([FromQuery] string userId)
        {
            var result = await _mediator.Send(new Applications.Admin.overview.Queries.GetTopStoreQuery { UserId = userId });
            return StatusCodeFromResult(result);
        }

        [HttpGet("overview/user-activity")]
        public async Task<IActionResult> GetUserActivity([FromQuery] string userId)
        {
            var result = await _mediator.Send(new Applications.Admin.overview.Queries.GetUserActivityQuery { UserId = userId });
            return StatusCodeFromResult(result);
        }

        [HttpGet("overview/day-sales")]
        public async Task<IActionResult> GetDayOfWeekSalesChart([FromQuery] string userId)
        {
            var result = await _mediator.Send(new Applications.Admin.overview.Queries.GetDayOfWeekSalesChartQuery { UserId = userId });
            return StatusCodeFromResult(result);
        }

        [HttpGet("overview/monthly-sales")]
        public async Task<IActionResult> GetMonthlySalesChart([FromQuery] string userId)
        {
            var result = await _mediator.Send(new Applications.Admin.overview.Queries.GetMonthlySalesChartQuery { UserId = userId });
            return StatusCodeFromResult(result);
        }

        // USER MANAGEMENT
        [HttpPost("user/list")]
        public async Task<IActionResult> GetUsers([FromBody] Applications.Admin.manage_user.Queries.GetUsersQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpPut("user/update")]
        public async Task<IActionResult> UpdateUser([FromBody] Applications.Admin.manage_user.Queries.UpdateUserCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCodeFromResult(result);
        }

        // SELLER MANAGEMENT
        [HttpGet("seller/list")]
        public async Task<IActionResult> GetSellers([FromBody] Applications.Admin.manage_seller.Queries.GetStoreListQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpPut("seller/update-status")]
        public async Task<IActionResult> UpdateSellerStatus([FromBody] Applications.Admin.manage_seller.Queries.UpdateStatusStoreCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCodeFromResult(result);
        }

        [HttpGet("seller/overview")]
        public async Task<IActionResult> GetSellerOverview([FromQuery] string userId)
        {
            var result = await _mediator.Send(new Applications.Admin.manage_seller.Queries.GetOverviewQuery { UserId = userId });
            return StatusCodeFromResult(result);
        }

        // PRODUCT MANAGEMENT
        [HttpGet("product/list")]
        public async Task<IActionResult> GetProducts([FromBody] Applications.Admin.manage_product.Queries.GetProductsQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpPut("product/update")]
        public async Task<IActionResult> UpdateProduct([FromBody] Applications.Admin.manage_product.Queries.UpdateProductCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCodeFromResult(result);
        }

        [HttpGet("product/overview")]
        public async Task<IActionResult> GetProductOverview([FromQuery] string userId)
        {
            var result = await _mediator.Send(new Applications.Admin.manage_product.Queries.GetOverviewQuery { UserId = userId });
            return StatusCodeFromResult(result);
        }

        [HttpGet("product/categories")]
        public async Task<IActionResult> GetCategories()
        {
            var result = await _mediator.Send(new Applications.Admin.manage_product.Queries.GetCategoiesQuery());
            return StatusCodeFromResult(result);
        }

        // ORDER MANAGEMENT
        [HttpGet("order/list")]
        public async Task<IActionResult> GetOrders([FromBody] Applications.Admin.manage_order.Queries.GetOrdersQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpGet("order/overview")]
        public async Task<IActionResult> GetOrderOverview([FromQuery] string userId)
        {
            var result = await _mediator.Send(new Applications.Admin.manage_order.Queries.GetOrderOverviewQuery { UserId = userId });
            return StatusCodeFromResult(result);
        }

        // REVENUE MANAGEMENT
        [HttpGet("revenue/overview")]
        public async Task<IActionResult> GetRevenueOverview([FromQuery] string userId)
        {
            var result = await _mediator.Send(new Applications.Admin.manage_revenue.Queries.GetOverviewRevenueQuery { UserId = userId });
            return StatusCodeFromResult(result);
        }

        [HttpGet("revenue/products")]
        public async Task<IActionResult> GetProductsWithRevenue([FromQuery] string userId)
        {
            var result = await _mediator.Send(new Applications.Admin.manage_revenue.Queries.GetProductsWithRevenueQuery { UserId = userId });
            return StatusCodeFromResult(result);
        }

        [HttpGet("revenue/analysis")]
        public async Task<IActionResult> GetRevenueAnalysis([FromQuery] string userId)
        {
            var result = await _mediator.Send(new Applications.Admin.manage_revenue.Queries.GetRevenueAnalysisQuery { UserId = userId });
            return StatusCodeFromResult(result);
        }

        [HttpGet("revenue/day-sales")]
        public async Task<IActionResult> GetRevenueDaySales([FromQuery] string userId)
        {
            var result = await _mediator.Send(new Applications.Admin.manage_revenue.Queries.GetDayOfWeekSalesChartQuery { UserId = userId });
            return StatusCodeFromResult(result);
        }

        [HttpGet("revenue/monthly-sales")]
        public async Task<IActionResult> GetRevenueMonthlySales([FromQuery] string userId)
        {
            var result = await _mediator.Send(new Applications.Admin.manage_revenue.Queries.GetMonthlySalesChartQuery { UserId = userId });
            return StatusCodeFromResult(result);
        }

        [HttpGet("revenue/top-store")]
        public async Task<IActionResult> GetRevenueTopStore([FromQuery] string userId)
        {
            var result = await _mediator.Send(new Applications.Admin.manage_revenue.Queries.GetTopStoreQuery { UserId = userId });
            return StatusCodeFromResult(result);
        }

        // CATEGORY MANAGEMENT
        [HttpPost("category/add")]
        public async Task<IActionResult> AddCategory([FromBody] Applications.Admin.manage_category.Queries.AddCategoryCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCodeFromResult(result);
        }

        [HttpPost("category/update")]
        public async Task<IActionResult> UpdateCategory([FromBody] Applications.Admin.manage_category.Queries.UpdateCategoryCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCodeFromResult(result);
        }

        [HttpGet("category/list")]
        public async Task<IActionResult> GetCategories([FromBody] Applications.Admin.manage_category.Queries.GetCategoryQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpGet("category/overview")]
        public async Task<IActionResult> GetCategoryOverview([FromQuery] string userId)
        {
            var result = await _mediator.Send(new Applications.Admin.manage_category.Queries.GetOverviewCategoryQuery { UserId = userId });
            return StatusCodeFromResult(result);
        }
    
        private IActionResult StatusCodeFromResult<T>(ServiceResult<T> result)
        {
            return result.Status switch
            {
                StatusResult.Success => Ok(result),
                StatusResult.Warning => Ok(result),
                StatusResult.Failed => NotFound(result),
                StatusResult.Errored => StatusCode(500, result),
                _ => StatusCode(500, result),
            };
        }
    }
}
