using DPTS.Applications.Dtos.auths;
using DPTS.Applications.Interfaces;
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

        /// <summary>
        /// Đăng ký tài khoản mới.
        /// </summary>
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Dữ liệu đăng ký không hợp lệ.");

            var result = await _authService.RegisterAsync(model);
            return APIResult.From(result);
        }

        /// <summary>
        /// Đăng nhập tài khoản (có thể trả về yêu cầu xác thực 2FA).
        /// </summary>
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Thông tin đăng nhập không hợp lệ.");

            var result = await _authService.LoginAsync(model);
            return APIResult.From(result);
        }

        /// <summary>
        /// Xác thực hai yếu tố (2FA).
        /// </summary>
        [HttpPost("2fa")]
        public async Task<IActionResult> Authenticate2FA([FromBody] Auth2FAModel model)
        {
            if (!ModelState.IsValid)
                return BadRequest("Mã xác thực không hợp lệ.");

            var result = await _authService.Auth2FAAsync(model);
            return APIResult.From(result);
        }
    }
}
