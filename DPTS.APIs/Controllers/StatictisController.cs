using DPTS.Applications.Interfaces;
using DPTS.Applications.Shareds;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace DPTS.APIs.Controllers
{
    [Route("/")]
    [ApiController]
    public class StatictisController : ControllerBase
    {
        private readonly IStatictisAdminService _statictisAdminService;
        private readonly IStatisticSellerService _statictisSellerService;
        private readonly ILogger<StatictisController> _logger;

        public StatictisController(
            IStatictisAdminService statictisAdminService,
            IStatisticSellerService statictisSellerService,
            ILogger<StatictisController> logger)
        {
            _statictisAdminService = statictisAdminService;
            _statictisSellerService = statictisSellerService;
            _logger = logger;
        }

        // ========== ADMIN ==========

        [HttpGet("admin/members")]
        public async Task<IActionResult> GetAllMembersAsync()
        {
            var result = await _statictisAdminService.GetMembersAsync();
            return APIResult.From(result);
        }

        [HttpGet("admin/members/buyer")]
        public async Task<IActionResult> GetBuyersAsync()
        {
            var result = await _statictisAdminService.GetMembersInBuyerRole();
            return APIResult.From(result);
        }

        [HttpGet("admin/members/seller")]
        public async Task<IActionResult> GetSellersAsync()
        {
            var result = await _statictisAdminService.GetMembersInSellerRole();
            return APIResult.From(result);
        }

        [HttpGet("admin/members/admin")]
        public async Task<IActionResult> GetAdminsAsync()
        {
            var result = await _statictisAdminService.GetMembersInAdminRole();
            return APIResult.From(result);
        }

        // ========== SELLER ==========

        [HttpGet("seller/{sellerId}/revenue")]
        public async Task<IActionResult> GetSalesRevenueAsync(string sellerId)
        {
            var result = await _statictisSellerService.SalesRevenueAsync(sellerId);
            return APIResult.From(result);
        }

        [HttpGet("seller/{sellerId}/orders")]
        public async Task<IActionResult> GetSoldOrdersAsync(string sellerId)
        {
            var result = await _statictisSellerService.SoldOrdersAsync(sellerId);
            return APIResult.From(result);
        }

        [HttpGet("seller/{sellerId}/products")]
        public async Task<IActionResult> GetProductStatisticAsync(string sellerId)
        {
            var result = await _statictisSellerService.ProductStatisticAsync(sellerId);
            return APIResult.From(result);
        }

        [HttpGet("seller/{sellerId}/rated")]
        public async Task<IActionResult> GetRatedAsync(string sellerId)
        {
            var result = await _statictisSellerService.RatedAsync(sellerId);
            return APIResult.From(result);
        }

        [HttpGet("seller/{sellerId}/recent-orders")]
        public async Task<IActionResult> GetRecentOrdersAsync(string sellerId)
        {
            var result = await _statictisSellerService.RecentOrderAsync(sellerId);
            return APIResult.From(result);
        }

        [HttpGet("seller/{sellerId}/recent-contacts")]
        public async Task<IActionResult> GetRecentContactsAsync(string sellerId)
        {
            var result = await _statictisSellerService.RecentContantAsync(sellerId);
            return APIResult.From(result);
        }

        [HttpGet("seller/{sellerId}/bestsellers")]
        public async Task<IActionResult> GetBestSellersAsync(string sellerId)
        {
            var result = await _statictisSellerService.BestSellerAsync(sellerId);
            return APIResult.From(result);
        }
    }
}
