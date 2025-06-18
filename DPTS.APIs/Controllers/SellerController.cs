using DPTS.Applications.Seller.conplaints.Queries;
using DPTS.Applications.Seller.overviews.Queries;
using DPTS.Applications.Seller.products.Queries;
using DPTS.Applications.Seller.revenues.Queries;
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

        [HttpGet("overview")]
        public async Task<IActionResult> GetOverview([FromQuery] GetSellerOverviewQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("revenues/chart")]
        public async Task<IActionResult> GetRevenueChart([FromQuery] GetRevenueChartQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("revenues/overview")]
        public async Task<IActionResult> GetRevenueOverview([FromQuery] GetRevenueOverviewQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("products/top-selling")]
        public async Task<IActionResult> GetTopSellingProducts([FromQuery] GetTopSellingProductsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("products")]
        public async Task<IActionResult> GetSellerProducts([FromQuery] GetSellerProductsItemQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("complaints")]
        public async Task<IActionResult> GetComplaints([FromQuery] GetComplaintsQuery query)
        {
            var result = await _mediator.Send(query);
            return Ok(result);
        }
    }
}
