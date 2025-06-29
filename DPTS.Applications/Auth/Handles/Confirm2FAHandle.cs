using DPTS.Applications.Auth.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Auth.Handles
{
    public class Confirm2FAHandle : IRequestHandler<Comfirm2FAQuery, ServiceResult<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserSecurityRepository _userSecurityRepository;
        private readonly ILogRepository _logRepository;
        private readonly ILogger<Confirm2FAHandle> _logger;

        public Confirm2FAHandle(
            IUserRepository userRepository,
            IUserSecurityRepository userSecurityRepository,
            ILogRepository logRepository,
            ILogger<Confirm2FAHandle> logger)
        {
            _userRepository = userRepository;
            _userSecurityRepository = userSecurityRepository;
            _logRepository = logRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<string>> Handle(Comfirm2FAQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Confirming 2FA for email: {Email}", request.Email);

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("User not found with email: {Email}", request.Email);
                return ServiceResult<string>.Error("Không tìm thấy người dùng.");
            }

            var security = await _userSecurityRepository.GetByUserIdAsync(user.UserId);
            if (security == null)
            {
                _logger.LogWarning("UserSecurity not found for userId: {UserId}", user.UserId);
                return ServiceResult<string>.Error("Thông tin bảo mật không hợp lệ.");
            }

            if (security.TwoFactorSecret != request.SecretCode)
            {
                _logger.LogWarning("Invalid 2FA code for userId: {UserId}", user.UserId);
                return ServiceResult<string>.Error("Mã xác thực không đúng.");
            }

            var log = new Log
            {
                LogId = Guid.NewGuid().ToString(),
                UserId = user.UserId,
                Action = "Confirm2FA",
                CreatedAt = DateTime.UtcNow,
                TargetId = user.UserId,
                TargetType = "User",
                IpAddress = "",
                UserAgent = "",
                UserType = "User"
            };

            try
            {
                security.TwoFactorSecret = string.Empty;
                security.TwoFactorEnabled = true;
                security.EmailVerified = true;

                await _userSecurityRepository.UpdateAsync(security);
                await _logRepository.AddAsync(log);

                _logger.LogInformation("2FA confirmed successfully for userId: {UserId}", user.UserId);
                return ServiceResult<string>.Success("Xác thực hai bước thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error confirming 2FA for userId: {UserId}", user.UserId);
                return ServiceResult<string>.Error("Lỗi hệ thống khi xác nhận mã xác thực.");
            }
        }
    }
}
