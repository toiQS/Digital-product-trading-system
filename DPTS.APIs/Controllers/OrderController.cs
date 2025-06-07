using DPTS.Applications.Interfaces;
using DPTS.APIs.Models;
using DPTS.Applications.Shareds;
using Microsoft.AspNetCore.Mvc;
using DPTS.Domains;

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
        public async Task<IActionResult> GetOrdersOfSeller([FromQuery] OrderModel model)
        {
            if (string.IsNullOrWhiteSpace(model.SellerId))
                return BadRequest("SellerId không được để trống.");

            var pageNumber = model.PageNumber > 0 ? model.PageNumber : 1;
            var pageSize = model.PageSize > 0 ? model.PageSize : 10;

            var result = await _orderService.GetOrdersBySellerId(model.SellerId, pageNumber, pageSize);
            return HandleResult(result);
        }

        [HttpPost("range-time")]
        public async Task<IActionResult> GetOrdersInRangeTime([FromBody] OrderOptionTimeModel model)
        {
            if (string.IsNullOrWhiteSpace(model.SellerId))
                return BadRequest("SellerId không được để trống.");

            if (!model.IsDay && !model.IsWeek && !model.IsMonth && !model.IsYear)
                return BadRequest("Phải chọn ít nhất một mốc thời gian.");

            var result = await _orderService.GetSoldOrderInRangeTimeAsync(
                model.SellerId, model.IsDay, model.IsWeek, model.IsMonth, model.IsYear
            );

            return HandleResult(result);
        }

        [HttpPost("filter")]
        public async Task<IActionResult> GetOrdersWithManyConditions([FromBody] GetOrdersWithManyConditionModel model)
        {
            if (string.IsNullOrWhiteSpace(model.SellerId))
                return BadRequest("SellerId không được để trống.");

            if (model.From > model.To)
                return BadRequest("Thời gian không hợp lệ: 'From' phải nhỏ hơn hoặc bằng 'To'.");

            var pageNumber = model.PageNumber > 0 ? model.PageNumber : 1;
            var pageSize = model.PageSize > 0 ? model.PageSize : 10;

            var result = await _orderService.GetOrderWithManyConditionAsync(
                model.From, model.To, pageNumber, pageSize, model.SellerId, model.Text, model.Status
            );

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
