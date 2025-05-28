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

        /// <summary>
        /// Lấy thông tin chi tiết người dùng theo ID.
        /// </summary>
        /// <param name="userId">ID người dùng</param>
        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUserById(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("UserId không hợp lệ.");

            var result = await _userService.GetUser(userId);
            return APIResult.From(result);
        }

        /// <summary>
        /// Gán hoặc hủy vai trò của người dùng.
        /// </summary>
        /// <param name="userId">ID người dùng</param>
        /// <param name="roleKey">Role cần cập nhật (ví dụ: seller, buyer)</param>
        /// <param name="isId">Xác định nếu `roleKey` là roleId thay vì roleName</param>
        [HttpPatch("{userId}/roles")]
        public async Task<IActionResult> PatchUserRole(string userId, [FromQuery] string roleKey, [FromQuery] bool isId = false)
        {
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(roleKey))
                return BadRequest("Thông tin đầu vào không hợp lệ.");

            var result = await _userService.PatchRoleOfUser(userId, roleKey, isId);
            return APIResult.From(result);
        }
    }
}
