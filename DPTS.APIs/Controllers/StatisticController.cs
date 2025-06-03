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

        [HttpGet("sales-revenue")]
        public async Task<IActionResult> SalesRevenue([FromQuery] StatisticOfSeller model)
        {
            if (string.IsNullOrWhiteSpace(model.SellerId))
                return BadRequest("SellerId không được để trống.");

            var result = await _statisticService.SalesRevenueInWeekAsync(
                model.SellerId, model.PageNumber, model.PageSize);

            return HandleResult(result);
        }

        [HttpGet("sold-orders")]
        public async Task<IActionResult> SoldOrders([FromQuery] StatisticOfSeller model)
        {
            if (string.IsNullOrWhiteSpace(model.SellerId))
                return BadRequest("SellerId không được để trống.");

            var result = await _statisticService.SoldOrdersInWeekAsync(
                model.SellerId, model.PageNumber, model.PageSize);

            return HandleResult(result);
        }

        [HttpGet("ratings")]
        public async Task<IActionResult> Ratings([FromQuery] StatisticOfSeller model)
        {
            if (string.IsNullOrWhiteSpace(model.SellerId))
                return BadRequest("SellerId không được để trống.");

            var result = await _statisticService.RatingAsync(
                model.SellerId, model.PageSize, model.PageNumber);

            return HandleResult(result);
        }

        [HttpGet("products-of-week")]
        public async Task<IActionResult> ProductsOfSeller([FromQuery] StatisticOfSeller model)
        {
            if (string.IsNullOrWhiteSpace(model.SellerId))
                return BadRequest("SellerId không được để trống.");

            var result = await _statisticService.ProductOfSellerInWeekAsync(
                model.SellerId, model.PageSize, model.PageNumber);

            return HandleResult(result);
        }

        [HttpGet("best-sell")]
        public async Task<IActionResult> BestSell([FromQuery] StatisticOfSeller model)
        {
            if (string.IsNullOrWhiteSpace(model.SellerId))
                return BadRequest("SellerId không được để trống.");

            var result = await _statisticService.BestSellAsync(
                model.SellerId, model.PageSize, model.PageNumber);

            return HandleResult(result);
        }

        [HttpGet("recent-messages")]
        public async Task<IActionResult> RecentMessages([FromQuery] StatisticOfSeller model)
        {
            if (string.IsNullOrWhiteSpace(model.SellerId))
                return BadRequest("SellerId không được để trống.");

            var result = await _statisticService.RecentMessageAsync(
                model.SellerId, model.PageNumber, model.PageSize);

            return HandleResult(result);
        }

        [HttpGet("recent-orders")]
        public async Task<IActionResult> RecentOrders([FromQuery] StatisticOfSeller model)
        {
            if (string.IsNullOrWhiteSpace(model.SellerId))
                return BadRequest("SellerId không được để trống.");

            var result = await _statisticService.RecentOrderAsync(
                model.SellerId, model.PageNumber, model.PageSize);

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
