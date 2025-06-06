using DPTS.Applications.Interfaces;
using DPTS.APIs.Models;
using DPTS.Applications.Shareds;
using Microsoft.AspNetCore.Mvc;

namespace DPTS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("seller")]
        public async Task<IActionResult> GetOrdersOfSeller([FromQuery] GetOrdersOfSellerModel model)
        {
            if (string.IsNullOrWhiteSpace(model.SellerId))
                return BadRequest("SellerId không được để trống.");

            var pageNumber = model.PageNumber > 0 ? model.PageNumber : 1;
            var pageSize = model.PageSize > 0 ? model.PageSize : 10;

            var result = await _orderService.GetOrdersBySellerId(model.SellerId, pageNumber, pageSize);
            return HandleResult(result);
        }

        [HttpGet("sold/day")]
        public async Task<IActionResult> GetSoldOrdersInDay([FromQuery] string sellerId)
        {
            if (string.IsNullOrWhiteSpace(sellerId))
                return BadRequest("SellerId không được để trống.");

            var result = await _orderService.SoldOrdersInDayAsync(sellerId);
            return HandleResult(result);
        }

        [HttpGet("sold/week")]
        public async Task<IActionResult> GetSoldOrdersInWeek([FromQuery] string sellerId)
        {
            if (string.IsNullOrWhiteSpace(sellerId))
                return BadRequest("SellerId không được để trống.");

            var result = await _orderService.SoldOrdersInWeekAsync(sellerId);
            return HandleResult(result);
        }

        [HttpGet("sold/month")]
        public async Task<IActionResult> GetSoldOrdersInMonth([FromQuery] string sellerId)
        {
            if (string.IsNullOrWhiteSpace(sellerId))
                return BadRequest("SellerId không được để trống.");

            var result = await _orderService.SoldOrdersInMonthAsync(sellerId);
            return HandleResult(result);
        }

        [HttpGet("sold/year")]
        public async Task<IActionResult> GetSoldOrdersInYear([FromQuery] string sellerId)
        {
            if (string.IsNullOrWhiteSpace(sellerId))
                return BadRequest("SellerId không được để trống.");

            var result = await _orderService.SoldOrdersInYearAsync(sellerId);
            return HandleResult(result);
        }

        [HttpGet("recent")]
        public async Task<IActionResult> GetRecentOrders([FromQuery] GetOrdersOfSellerModel model)
        {
            if (string.IsNullOrWhiteSpace(model.SellerId))
                return BadRequest("SellerId không được để trống.");

            var pageNumber = model.PageNumber > 0 ? model.PageNumber : 1;
            var pageSize = model.PageSize > 0 ? model.PageSize : 10;

            var result = await _orderService.RecentOrderAsync(model.SellerId, pageNumber, pageSize);
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
