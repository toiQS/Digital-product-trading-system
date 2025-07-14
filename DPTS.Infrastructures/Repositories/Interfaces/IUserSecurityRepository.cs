using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Interfaces
{
    public interface IUserSecurityRepository
    {
        #region Query

        // Lấy thông tin bảo mật của user
        Task<UserSecurity?> GetByUserIdAsync(string userId);

        // Kiểm tra tài khoản có bị lock không
        Task<bool> IsLockedOutAsync(string userId);

        // Lấy thời gian lockout (nếu có)
        Task<DateTime?> GetLockoutUntilAsync(string userId);

        // Kiểm tra xem tài khoản đã xác minh email chưa
        Task<bool> IsEmailVerifiedAsync(string userId);

        // Kiểm tra trạng thái bật/tắt 2FA
        Task<bool> IsTwoFactorEnabledAsync(string userId);

        #endregion

        #region Crud

        Task AddAsync(UserSecurity security);
        Task UpdateAsync(UserSecurity security);
        Task RemoveAsync(UserSecurity security);

        #endregion

        #region Login Handling

        // Cập nhật số lần đăng nhập thất bại
        Task IncrementFailedLoginAttemptsAsync(string userId);

        // Reset số lần thất bại về 0
        Task ResetFailedLoginAttemptsAsync(string userId);

        // Đặt lockout cho user (sau khi vượt quá số lần cho phép)
        Task LockoutUserAsync(string userId, DateTime until);

        // Xác minh email
        Task MarkEmailVerifiedAsync(string userId);

        // Cập nhật mật khẩu (hash mới)
        Task UpdatePasswordAsync(string userId, string newPasswordHash);

        // Cập nhật thông tin 2FA
        Task UpdateTwoFactorAsync(string userId, bool enabled, string? secret);

        #endregion
    }
}
