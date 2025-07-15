using DPTS.Applications.Buyer.Queries.chat;
using DPTS.Applications.Buyer.Queries.complaint;
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
using System.Threading.Tasks;

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

        // Chat
        [HttpPost("chat/get")]
        public async Task<IActionResult> GetChat([FromBody] GetChatQuery query)
            => StatusCodeFromResult(await _mediator.Send(query));

        [HttpPost("chat/send")]
        public async Task<IActionResult> SendMessage([FromBody] SendMessageToStoreCommand command)
            => StatusCodeFromResult(await _mediator.Send(command));

        // Complaint
        [HttpPost("complaint/add")]
        public async Task<IActionResult> AddComplaint([FromBody] AddComplaintCommand command)
            => StatusCodeFromResult(await _mediator.Send(command));

        [HttpPost("complaint/info")]
        public async Task<IActionResult> GetComplaintInfo([FromBody] InformationForComplaintQuery query)
            => StatusCodeFromResult(await _mediator.Send(query));

        // Order
        [HttpPost("order/add-to-cart")]
        public async Task<IActionResult> AddToCart([FromBody] AddToCartCommand command)
            => StatusCodeFromResult(await _mediator.Send(command));

        [HttpPost("order/check-buy-now")]
        public async Task<IActionResult> CheckBuyNow([FromBody] CheckBuyNowQuery query)
            => StatusCodeFromResult(await _mediator.Send(query));

        [HttpPost("order/checkout")]
        public async Task<IActionResult> GetCheckout([FromBody] GetCheckoutQuery query)
            => StatusCodeFromResult(await _mediator.Send(query));

        [HttpPost("order/detail")]
        public async Task<IActionResult> GetOrderDetail([FromBody] GetDetailOrderQuery query)
            => StatusCodeFromResult(await _mediator.Send(query));

        [HttpPost("order/purchased")]
        public async Task<IActionResult> GetPurchasedOrders([FromBody] GetPurchasedOrderQuery query)
            => StatusCodeFromResult(await _mediator.Send(query));

        [HttpPost("order/remove-product")]
        public async Task<IActionResult> RemoveProduct([FromBody] RemoveProductFormOrderCommand command)
            => StatusCodeFromResult(await _mediator.Send(command));

        // Payment
        [HttpPost("payment/result")]
        public async Task<IActionResult> GetPaymentResult([FromBody] GetPaymentResultQuery query)
            => StatusCodeFromResult(await _mediator.Send(query));

        // Product
        [HttpPost("product/detail")]
        public async Task<IActionResult> GetProductDetail([FromBody] GetProductDetailQuery query)
            => StatusCodeFromResult(await _mediator.Send(query));

        [HttpPost("product/list")]
        public async Task<IActionResult> GetProductIndexList([FromBody] GetProductIndexListQuery query)
            => StatusCodeFromResult(await _mediator.Send(query));

        // Profile
        [HttpPost("profile/mini")]
        public async Task<IActionResult> GetUserProfileMini([FromBody] GetUserProfileMiniQuery query)
            => StatusCodeFromResult(await _mediator.Send(query));

        [HttpPost("profile/detail")]
        public async Task<IActionResult> GetUserProfile([FromBody] GetUserProfileQuery query)
            => StatusCodeFromResult(await _mediator.Send(query));

        [HttpPut("profile/update")]
        public async Task<IActionResult> UpdateUserProfile([FromBody] UpdateUserProfileCommand command)
            => StatusCodeFromResult(await _mediator.Send(command));

        [HttpPut("profile/update-mini")]
        public async Task<IActionResult> UpdateUserProfileMini([FromBody] UpdateUserProfileMiniCommand command)
            => StatusCodeFromResult(await _mediator.Send(command));

        // Review
        [HttpPost("review/create")]
        public async Task<IActionResult> CreateReview([FromBody] CreateProductReviewCommand command)
            => StatusCodeFromResult(await _mediator.Send(command));

        [HttpPost("review/like")]
        public async Task<IActionResult> LikeComment([FromBody] LikeCommentCommand command)
            => StatusCodeFromResult(await _mediator.Send(command));

        [HttpPost("review/unlike")]
        public async Task<IActionResult> UnlikeComment([FromBody] UnlikeCommentCommand command)
            => StatusCodeFromResult(await _mediator.Send(command));

        // Security
        [HttpPut("security/change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordCommand command)
            => StatusCodeFromResult(await _mediator.Send(command));

        // Wallet
        [HttpPost("wallet/get")]
        public async Task<IActionResult> GetWallet([FromBody] GetWalletQuery query)
            => StatusCodeFromResult(await _mediator.Send(query));
    }
}
