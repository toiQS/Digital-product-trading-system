using DPTS.Applications.Buyers.profiles.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyers.profiles.Handles
{
    public class ChangePasswordHandler : IRequestHandler<ChangePasswordCommand, ServiceResult<string>>
    {
        private readonly ILogger<ChangePasswordHandler> _logger;
        private readonly IUserRepository _userRepository;
        private readonly ILogRepository _logRepository;

        public ChangePasswordHandler(
            ILogger<ChangePasswordHandler> logger,
            IUserRepository userRepository,
            ILogRepository logRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _logRepository = logRepository;
        }

        public async Task<ServiceResult<string>> Handle(ChangePasswordCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Yêu cầu đổi mật khẩu từ UserId: {UserId}", request.UserId);

            if (request.NewPassword != request.ConfirmNewPassword)
            {
                _logger.LogWarning("Xác nhận mật khẩu mới không trùng khớp.");
                return ServiceResult<string>.Error("Xác nhận mật khẩu không trùng khớp.");
            }

            try
            {
                var user = await _userRepository.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    _logger.LogWarning("Không tìm thấy người dùng với ID: {UserId}", request.UserId);
                    return ServiceResult<string>.Error("Người dùng không tồn tại.");
                }

                if (user.PasswordHash != request.CurrentPassword)
                {
                    _logger.LogWarning("Mật khẩu hiện tại không đúng với UserId: {UserId}", request.UserId);
                    return ServiceResult<string>.Error("Mật khẩu hiện tại không chính xác.");
                }

                user.PasswordHash = request.NewPassword;
                user.UpdatedAt = DateTime.UtcNow;

                var log = new Log
                {
                    LogId = Guid.NewGuid().ToString(),
                    UserId = request.UserId,
                    Action = $"Người dùng {request.UserId} đã thay đổi mật khẩu",
                    CreatedAt = DateTime.UtcNow
                };

                try
                {
                    await _userRepository.UpdateAsync(user);
                    await _logRepository.AddAsync(log);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi cập nhật mật khẩu cho UserId: {UserId}", request.UserId);
                    return ServiceResult<string>.Error("Đã xảy ra lỗi khi lưu mật khẩu mới.");
                }

                _logger.LogInformation("Đổi mật khẩu thành công cho UserId: {UserId}", request.UserId);
                return ServiceResult<string>.Success("Mật khẩu đã được cập nhật.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi hệ thống khi xử lý đổi mật khẩu cho UserId: {UserId}", request.UserId);
                return ServiceResult<string>.Error("Có lỗi xảy ra. Vui lòng thử lại sau.");
            }
        }
    }
}
