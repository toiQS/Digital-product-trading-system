using DPTS.Applications.Auth.Dtos;
using DPTS.Applications.Auth.Queries;
using DPTS.Applications.Shareds;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace DPTS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpPost("enable-2fa")]
        public async Task<IActionResult> Enable2FA([FromBody] Enable2FAQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
        }

        [HttpPost("confirm-2fa")]
        public async Task<IActionResult> Confirm2FA([FromBody] Comfirm2FAQuery query)
        {
            var result = await _mediator.Send(query);
            return StatusCodeFromResult(result);
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
    }
}
