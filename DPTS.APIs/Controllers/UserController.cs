using DPTS.APIs.Models;
using DPTS.Applications.Interfaces;
using DPTS.Applications.Shareds;
using Microsoft.AspNetCore.Mvc;

namespace DPTS.APIs.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> GetUser([FromQuery] UserModel model)
        {
            if (string.IsNullOrWhiteSpace(model.UserId))
                return BadRequest("UserId không được để trống.");

            var result = await _userService.GetUserAsync(model.UserId);
            return HandleResult(result);
        }

        [HttpGet("mini-profile")]
        public async Task<IActionResult> GetMiniProfile([FromQuery] UserModel model)
        {
            if (string.IsNullOrWhiteSpace(model.UserId))
                return BadRequest("UserId không được để trống.");

            var result = await _userService.GetMiniProfileAsync(model.UserId);
            return HandleResult(result);
        }

        [HttpPatch("role")]
        public async Task<IActionResult> PatchRole([FromBody] PatchRoleOfUserAsync model, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(model.AdminUserId) || string.IsNullOrWhiteSpace(model.UserId))
                return BadRequest("AdminUserId và UserId không được để trống.");

            var result = await _userService.PatchRoleOfUserAsync(
                model.AdminUserId,
                model.UserId,
                model.IsBuyer,
                model.IsAdmin,
                cancellationToken);

            return HandleResult(result);
        }

        [HttpPost("change-password")]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordModel model)
        {
            if (string.IsNullOrWhiteSpace(model.UserId)
                || string.IsNullOrWhiteSpace(model.Password)
                || string.IsNullOrWhiteSpace(model.NewPassword)
                || string.IsNullOrWhiteSpace(model.NewPasswordComfirmed))
            {
                return BadRequest("Tất cả các trường bắt buộc phải được cung cấp.");
            }

            var result = await _userService.ChangePasswordAsync(
                model.UserId,
                model.Password,
                model.NewPassword,
                model.NewPasswordComfirmed);

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
