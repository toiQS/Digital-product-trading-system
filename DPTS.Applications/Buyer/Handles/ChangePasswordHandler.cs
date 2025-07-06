using DPTS.Applications.Auth.Handles;
using DPTS.Applications.Buyer.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles;

/// <summary>
/// Xử lý thay đổi mật khẩu cho người dùng.
/// </summary>
public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, ServiceResult<string>>
{
    private readonly ILogger<ChangePasswordHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUserSecurityRepository _userSecurityRepository;

    public ChangePasswordHandler(
        ILogger<ChangePasswordHandler> logger,
        IUserRepository userRepository,
        ILogRepository logRepository,
        IUserSecurityRepository userSecurityRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _logRepository = logRepository;
        _userSecurityRepository = userSecurityRepository;
    }

    public async Task<ServiceResult<string>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling ChangePasswordCommand for user: {UserId}", request.UserId);

        // 1. Xác nhận mật khẩu mới và mật khẩu xác nhận có khớp không
        if (request.NewPassword != request.ConfirmNewPassword)
        {
            _logger.LogWarning("New password and confirm password do not match.");
            return ServiceResult<string>.Error("Mật khẩu mới và xác nhận không khớp.");
        }

        // 2. Kiểm tra user có tồn tại không
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", request.UserId);
            return ServiceResult<string>.Error("Không tìm thấy người dùng.");
        }

        // 3. Lấy thông tin bảo mật (chứa mật khẩu)
        var security = await _userSecurityRepository.GetByUserIdAsync(request.UserId);
        if (security == null)
        {
            _logger.LogError("User security info not found: {UserId}", request.UserId);
            return ServiceResult<string>.Error("Không tìm thấy thông tin bảo mật của người dùng.");
        }

        // 4. Kiểm tra mật khẩu hiện tại có đúng không
        if (!PasswordHasher.Verify(request.CurrentPassword, security.PasswordHash))
        {
            _logger.LogWarning("Current password is incorrect for user: {UserId}", request.UserId);
            return ServiceResult<string>.Error("Mật khẩu hiện tại không chính xác.");
        }

        // 5. Tiến hành cập nhật mật khẩu mới
        try
        {
            security.PasswordHash = PasswordHasher.Hash(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            await _userSecurityRepository.UpdateAsync(security);
            await _userRepository.UpdateAsync(user);

            // Ghi log thay đổi
            Log log = new()
            {
                LogId = Guid.NewGuid().ToString(),
                UserId = user.UserId,
                Action = "ChangePassword",
                TargetId = user.UserId,
                TargetType = "User",
                CreatedAt = DateTime.UtcNow
            };
            await _logRepository.AddAsync(log);

            return ServiceResult<string>.Success("Thay đổi mật khẩu thành công.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to change password for user: {UserId}", request.UserId);
            return ServiceResult<string>.Error("Đã xảy ra lỗi khi thay đổi mật khẩu. Vui lòng thử lại sau.");
        }
    }
}
