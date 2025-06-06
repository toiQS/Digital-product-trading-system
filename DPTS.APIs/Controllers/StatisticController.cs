using DPTS.Applications.Interfaces;
using DPTS.Applications.Shareds;
using DPTS.APIs.Models;
using Microsoft.AspNetCore.Mvc;

namespace DPTS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatisticController : ControllerBase
    {
        private readonly IStatisticService _statisticService;

        public StatisticController(IStatisticService statisticService)
        {
            _statisticService = statisticService;
        }

        // ---------------- Doanh thu ----------------
        [HttpGet("revenue/day")]
        public async Task<IActionResult> GetRevenueDay([FromQuery] StatisticOfSeller model)
        {
            if (string.IsNullOrWhiteSpace(model.SellerId))
                return BadRequest("SellerId không được để trống.");

            var result = await _statisticService.SalesRevenueInDayAsync(model.SellerId);
            return HandleResult(result);
        }

        [HttpGet("revenue/week")]
        public async Task<IActionResult> GetRevenueWeek([FromQuery] StatisticOfSeller model)
        {
            if (string.IsNullOrWhiteSpace(model.SellerId))
                return BadRequest("SellerId không được để trống.");

            var result = await _statisticService.SalesRevenueInWeekAsync(model.SellerId);
            return HandleResult(result);
        }

        [HttpGet("revenue/month")]
        public async Task<IActionResult> GetRevenueMonth([FromQuery] StatisticOfSeller model)
        {
            if (string.IsNullOrWhiteSpace(model.SellerId))
                return BadRequest("SellerId không được để trống.");

            var result = await _statisticService.SalesRevenueInMonthAsync(model.SellerId);
            return HandleResult(result);
        }

        [HttpGet("revenue/year")]
        public async Task<IActionResult> GetRevenueYear([FromQuery] StatisticOfSeller model)
        {
            if (string.IsNullOrWhiteSpace(model.SellerId))
                return BadRequest("SellerId không được để trống.");

            var result = await _statisticService.SaleRevenueInYearAsync(model.SellerId);
            return HandleResult(result);
        }

        // ---------------- Đánh giá ----------------
        [HttpGet("rating")]
        public async Task<IActionResult> GetRating([FromQuery] StatisticOfSeller model)
        {
            if (string.IsNullOrWhiteSpace(model.SellerId))
                return BadRequest("SellerId không được để trống.");

            var result = await _statisticService.RatingAsync(model.SellerId);
            return HandleResult(result);
        }

        // ---------------- Sản phẩm ----------------
        [HttpGet("product/week")]
        public async Task<IActionResult> GetProductStatisticOfWeek([FromQuery] StatisticOfSeller model)
        {
            if (string.IsNullOrWhiteSpace(model.SellerId))
                return BadRequest("SellerId không được để trống.");

            var result = await _statisticService.ProductOfSellerInWeekAsync(model.SellerId);
            return HandleResult(result);
        }

        // ---------------- Shared Result Handler ----------------
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
