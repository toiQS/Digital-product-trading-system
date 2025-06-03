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
