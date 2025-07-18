using DPTS.Applications.Shareds;
using DPTS.Applications.Seller.Query.chat;
using DPTS.Applications.Seller.Query.complaint;
using DPTS.Applications.Seller.Query.dashboard;
using DPTS.Applications.Seller.Query.order;
using DPTS.Applications.Seller.Query.product;
using DPTS.Applications.Seller.Query.revenue;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace DPTS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SellerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public SellerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // -------------------- Chat --------------------
        [HttpGet("chat")]
        public async Task<IActionResult> GetChat([FromQuery] GetChatQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessageToBuyer([FromBody] SendMessageToBuyerCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCodeFromResult(result);
        }

        // -------------------- Complaint --------------------
        [HttpGet("complaints")]
        public async Task<IActionResult> GetComplaints([FromQuery] GetComplaintsQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpGet("complaint-detail")]
        public async Task<IActionResult> GetDetailComplaint([FromQuery] GetDetailComplaintQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpGet("complaint-overview")]
        public async Task<IActionResult> GetComplaintOverview([FromQuery] GetComplaintOverviewQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpPut("complaint-confirm-resolved")]
        public async Task<IActionResult> ConfirmResolvedComplaint([FromBody] ComfirmResovledComplaintCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCodeFromResult(result);
        }

        // -------------------- Dashboard --------------------
        [HttpGet("overview")]
        public async Task<IActionResult> GetSellerOverview([FromQuery] GetSellerOverviewQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        // -------------------- Order --------------------
        [HttpGet("orders")]
        public async Task<IActionResult> GetSellerOrders([FromQuery] GetSellerOrderQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpPut("order-confirm-send")]
        public async Task<IActionResult> ConfirmAndSendProduct([FromBody] ConfirmAndSendProductCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCodeFromResult(result);
        }

        // -------------------- Product --------------------
        [HttpPost("product-add")]
        public async Task<IActionResult> AddProduct([FromBody] AddProductCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCodeFromResult(result);
        }

        [HttpPut("product-update")]
        public async Task<IActionResult> UpdateProduct([FromBody] UpdateProductCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCodeFromResult(result);
        }

        [HttpDelete("product-delete")]
        public async Task<IActionResult> DeleteProduct([FromBody] DeleteProductCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCodeFromResult(result);
        }

        [HttpGet("product-detail")]
        public async Task<IActionResult> GetProductDetail([FromQuery] GetProductDetailQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpGet("product-items")]
        public async Task<IActionResult> GetSellerProductItems([FromQuery] GetSellerProductsItemQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpGet("top-selling-products")]
        public async Task<IActionResult> GetTopSellingProducts([FromQuery] GetTopSellingProductsQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpPost("product-image-add")]
        public async Task<IActionResult> AddProductImage([FromBody] AddProductImageCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCodeFromResult(result);
        }

        [HttpPut("product-image-update")]
        public async Task<IActionResult> UpdateProductImage([FromBody] UpdateProductImageCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCodeFromResult(result);
        }

        [HttpDelete("product-image-delete")]
        public async Task<IActionResult> DeleteProductImage([FromBody] DeleteProductImageCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCodeFromResult(result);
        }

        // -------------------- Revenue --------------------
        [HttpGet("revenue-overview")]
        public async Task<IActionResult> GetRevenueOverview([FromQuery] GetRevenueOverviewQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpGet("revenue-chart")]
        public async Task<IActionResult> GetRevenueChart([FromQuery] GetRevenueChartQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        // -------------------- Helper --------------------
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
