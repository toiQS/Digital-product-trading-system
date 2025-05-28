using DPTS.Applications.Interfaces;
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

        /// <summary>
        /// Lấy danh sách đơn hàng theo mã người bán.
        /// </summary>
        /// <param name="sellerId">Mã người bán</param>
        [HttpGet("seller/{sellerId}")]
        public async Task<IActionResult> GetOrdersBySeller(string sellerId)
        {
            if (string.IsNullOrWhiteSpace(sellerId))
                return BadRequest("Mã người bán không được để trống.");

            var result = await _orderService.GetOrdersBySellerId(sellerId);
            return APIResult.From(result);
        }
    }
}
