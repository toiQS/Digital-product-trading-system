using DPTS.Applications.Interfaces;
using DPTS.APIs.Models;
using DPTS.Applications.Shareds;
using Microsoft.AspNetCore.Mvc;

namespace DPTS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;
        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegistrationModel model, CancellationToken cancellationToken)
        {
            if (model.Password != model.PasswordComfirmed)
                return BadRequest("Mật khẩu xác nhận không khớp.");

            var result = await _authService.RegisterAsync(model.Email, model.Password, model.PasswordComfirmed, true, cancellationToken);

            return HandleServiceResult(result);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model, CancellationToken cancellationToken)
        {
            var result = await _authService.LoginAsync(model.Email, model.Password, cancellationToken);

            return HandleServiceResult(result);
        }

        [HttpPost("2fa")]
        public async Task<IActionResult> Auth2FA([FromBody] Auth2FAModel model, CancellationToken cancellationToken)
        {
            var result = await _authService.Auth2FAAsync(model.Email, model.TwoFactorSecret, cancellationToken);

            return HandleServiceResult(result);
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordModel model)
        {
            var result = await _authService.ForgotPasswordAsync(model.Email);
            return HandleServiceResult(result);
        }

        private IActionResult HandleServiceResult(ServiceResult<string> result)
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
