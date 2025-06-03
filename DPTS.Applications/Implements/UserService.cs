using DPTS.Applications.Dtos;
using DPTS.Applications.Interfaces;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Implements
{
    public class UserService : IUserService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<UserService> _logger;

        public UserService(ApplicationDbContext context, ILogger<UserService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Lấy thông tin hồ sơ người dùng chi tiết theo UserId.
        /// </summary>
        public async Task<ServiceResult<ProfileDto>> GetUserAsync(string userId)
        {
            _logger.LogInformation("Lấy thông tin người dùng với ID: {UserId}", userId);
            var user = await _context.Users.Include(u => u.Address).FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null) return ServiceResult<ProfileDto>.Error("Người dùng không tồn tại.");

            var profile = new ProfileDto()
            {
                UserId = userId,
                FullName = user.FullName!,
                Address = new AddressDto
                {
                    City = user.Address.City,
                    Country = user.Address.Country,
                    District = user.Address.District,
                    PostalCode = user.Address.PostalCode,
                    Street = user.Address.Street,
                },
                NumberPhone = user.Phone!,
                Email = user.Email,
            };
            return ServiceResult<ProfileDto>.Success(profile);
        }

        /// <summary>
        /// Lấy thông tin hồ sơ rút gọn theo UserId.
        /// </summary>
        public async Task<ServiceResult<MiniProfileDto>> GetMiniProfileAsync(string userId)
        {
            _logger.LogInformation("Lấy mini profile của người dùng {UserId}", userId);
            var user = await _context.Users.Include(u => u.Address).FirstOrDefaultAsync(x => x.UserId == userId);
            if (user == null) return ServiceResult<MiniProfileDto>.Error("Người dùng không tồn tại.");

            var profile = new MiniProfileDto()
            {
                UserId = user.UserId,
                FullName = user.FullName!,
                Email = user.Email,
                Image = user.ImageUrl
            };
            return ServiceResult<MiniProfileDto>.Success(profile);
        }

        /// <summary>
        /// Cập nhật vai trò của người dùng bởi quản trị viên.
        /// </summary>
        public async Task<ServiceResult<string>> PatchRoleOfUserAsync(string adminUserId, string userId, bool isBuyer = true, bool isAdmin = false, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Admin {AdminId} cập nhật vai trò người dùng {UserId}", adminUserId, userId);

            var admin = await _context.Users.FindAsync(adminUserId);
            if (admin == null) return ServiceResult<string>.Error("Admin không tồn tại.");

            var roleAdmin = await _context.Roles.FindAsync(admin.RoleId);
            if (roleAdmin?.RoleName != "Admin") return ServiceResult<string>.Error("Không có quyền cập nhật.");

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return ServiceResult<string>.Error("Người dùng không tồn tại.");

            var targetRoleName = isAdmin ? "Admin" : (isBuyer ? "Buyer" : "Seller");
            var targetRole = await _context.Roles.FirstOrDefaultAsync(x => x.RoleName == targetRoleName, cancellationToken);
            if (targetRole == null) return ServiceResult<string>.Error("Vai trò không hợp lệ.");

            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

                user.RoleId = targetRole.RoleId;
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync();

                var logContent = isAdmin
                    ? $"Người dùng {userId} {user.FullName} được cập nhật thành Admin."
                    : $"Người dùng {userId} {user.FullName} được cập nhật thành {(isBuyer ? "Buyer" : "Seller")}.";
                await AddLogAsync(userId, logContent);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật vai trò người dùng.");
                return ServiceResult<string>.Error("Cập nhật vai trò thất bại.");
            }

            return ServiceResult<string>.Success("Cập nhật vai trò thành công.");
        }

        /// <summary>
        /// Thay đổi mật khẩu người dùng.
        /// </summary>
        public async Task<ServiceResult<string>> ChangePasswordAsync(string userId, string oldPassword, string newPassword, string newPasswordComfirmed)
        {
            if (newPassword != newPasswordComfirmed)
                return ServiceResult<string>.Error("Mật khẩu xác nhận không khớp.");

            var user = await _context.Users.FindAsync(userId);
            if (user == null) return ServiceResult<string>.Error("Người dùng không tồn tại.");

            if (!BCrypt.Net.BCrypt.Verify(oldPassword, user.PasswordHash))
                return ServiceResult<string>.Error("Mật khẩu cũ không chính xác.");

            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                _context.Entry(user).State = EntityState.Modified;
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thay đổi mật khẩu.");
                return ServiceResult<string>.Error("Đổi mật khẩu thất bại.");
            }

            return ServiceResult<string>.Success("Đổi mật khẩu thành công.");
        }

        /// <summary>
        /// Ghi log hành động của người dùng.
        /// </summary>
        private async Task<ServiceResult<string>> AddLogAsync(string userId, string content)
        {
            var log = new Log
            {
                LogId = Guid.NewGuid().ToString(),
                Action = content,
                UserId = userId,
                CreatedAt = DateTime.Now,
            };

            try
            {
                using var transaction = await _context.Database.BeginTransactionAsync();
                _context.Logs.Add(log);
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi ghi log.");
                return ServiceResult<string>.Error("Không thể ghi log.");
            }

            return ServiceResult<string>.Success("Đã ghi log.");
        }
    }
}
