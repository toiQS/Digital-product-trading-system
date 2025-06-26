using DPTS.Applications.Buyers.payments.Dtos;
using DPTS.Applications.Buyers.payments.Queries;
using DPTS.Applications.Buyers.reviews.Queries;
using DPTS.Applications.Shareds;
using MediatR;
using Microsoft.AspNetCore.Http;
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

        /// <summary>
        /// Lấy thông tin trang thanh toán của buyer
        /// </summary>
        [HttpGet("checkout/{buyerId}")]
        [ProducesResponseType(typeof(ServiceResult<CheckoutDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetCheckoutAsync(string buyerId)
        {
            var result = await _mediator.Send(new GetCheckoutQuery { BuyerId = buyerId });
            return StatusCode(ConvertStatus(result.Status), result);
        }

        /// <summary>
        /// Lấy kết quả thanh toán của buyer
        /// </summary>
        [HttpGet("payment-result/{buyerId}")]
        [ProducesResponseType(typeof(ServiceResult<PaymentResultDto>), StatusCodes.Status200OK)]
        public async Task<IActionResult> GetPaymentResultAsync(string buyerId)
        {
            var result = await _mediator.Send(new GetPaymentResultQuery { BuyerId = buyerId });
            return StatusCode(ConvertStatus(result.Status), result);
        }

        /// <summary>
        /// Buyer tạo đánh giá sản phẩm
        /// </summary>
        [HttpPost("review")]
        [ProducesResponseType(typeof(ServiceResult<string>), StatusCodes.Status201Created)]
        public async Task<IActionResult> CreateReviewAsync([FromBody] CreateProductReviewCommand command)
        {
            var result = await _mediator.Send(command);
            return StatusCode(ConvertStatus(result.Status), result);
        }

        private int ConvertStatus(StatusResult status)
        {
            return status switch
            {
                StatusResult.Success => StatusCodes.Status200OK,
                StatusResult.Warning => StatusCodes.Status200OK,
                StatusResult.Failed => StatusCodes.Status404NotFound,
                StatusResult.Errored => StatusCodes.Status500InternalServerError,
                _ => StatusCodes.Status500InternalServerError
            };
        }
    }
}
