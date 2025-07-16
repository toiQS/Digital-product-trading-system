using DPTS.Applications.Buyer.Queries.chat;
using DPTS.Applications.Buyer.Queries.order;
using DPTS.Applications.Buyer.Queries.payment;
using DPTS.Applications.Buyer.Queries.product;
using DPTS.Applications.Buyer.Queries.profile;
using DPTS.Applications.Buyer.Queries.review;
using DPTS.Applications.Buyer.Queries.security;
using DPTS.Applications.Buyer.Queries.wallet;
using DPTS.Applications.Shareds;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace DPTS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BuyerController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BuyerController(IMediator mediator)
        {
            _mediator = mediator;
        }

        // --------------------- Cart ---------------------
        [HttpPost("add-to-cart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartCommand query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpPut("check-buy-now")]
        public async Task<IActionResult> CheckBuyNow([FromBody] CheckBuyNowQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpPut("remove-product")]
        public async Task<IActionResult> RemoveProductFromOrder([FromBody] RemoveProductFormOrderCommand query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        // --------------------- Product ---------------------
        [HttpGet("product-detail")]
        public async Task<IActionResult> GetProductDetail([FromQuery] GetProductDetailQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpGet("product-index")]
        public async Task<IActionResult> GetProductIndex([FromQuery] GetProductIndexListQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpPut("review-product")]
        public async Task<IActionResult> CreateProductReview([FromBody] CreateProductReviewCommand query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpPut("like-review")]
        public async Task<IActionResult> LikeReview([FromBody] LikeReviewCommand query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpPost("unlike-review")]
        public async Task<IActionResult> UnlikeReview([FromBody] UnlikeReviewCommand query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        // --------------------- Profile ---------------------
        [HttpGet("profile")]
        public async Task<IActionResult> GetUserProfile([FromQuery] GetUserProfileQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpGet("profile-mini")]
        public async Task<IActionResult> GetUserProfileMini([FromQuery] GetUserProfileMiniQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpPut("profile-update")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileCommand query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpPut("profile-mini-update")]
        public async Task<IActionResult> UpdateUserProfileMini([FromBody] UpdateUserProfileMiniCommand query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        // --------------------- Checkout ---------------------
        [HttpPut("checkout")]
        public async Task<IActionResult> GetCheckout([FromQuery] GetCheckoutQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        // --------------------- Order ---------------------
        [HttpGet("order-detail")]
        public async Task<IActionResult> GetOrderDetail([FromQuery] GetDetailOrderQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpGet("purchased-orders")]
        public async Task<IActionResult> GetPurchasedOrders([FromQuery] GetPurchasedOrderQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        // --------------------- Chat ---------------------
        [HttpGet("chat")]
        public async Task<IActionResult> GetChat([FromQuery] GetChatQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpPost("send-message")]
        public async Task<IActionResult> SendMessageToStore([FromBody] SendMessageToStoreCommand query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        // --------------------- Account ---------------------
        [HttpPut("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCodeFromResult(result);
        }

        // --------------------- Payment ---------------------
        [HttpGet("payment-result")]
        public async Task<IActionResult> GetPaymentResult([FromQuery] GetPaymentResultQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        // --------------------- Wallet ---------------------
        [HttpGet("wallet")]
        public async Task<IActionResult> GetWallet([FromQuery] GetWalletQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        // --------------------- Helper ---------------------
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
