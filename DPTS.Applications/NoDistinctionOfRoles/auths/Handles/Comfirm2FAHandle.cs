using DPTS.Applications.NoDistinctionOfRoles.auths.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.NoDistinctionOfRoles.auths.Handles
{
    public class Confirm2FAHandler : IRequestHandler<Comfirm2FAQuery, ServiceResult<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserSecurityRepository _userSecurityRepository;
        private readonly ILogRepository _logRepository;
        private readonly ILogger<Confirm2FAHandler> _logger;

        public Confirm2FAHandler(
            IUserRepository userRepository,
            IUserSecurityRepository userSecurityRepository,
            ILogRepository logRepository,
            ILogger<Confirm2FAHandler> logger)
        {
            _userRepository = userRepository;
            _userSecurityRepository = userSecurityRepository;
            _logRepository = logRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<string>> Handle(Comfirm2FAQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.SecretCode))
            {
                _logger.LogWarning("Email hoặc mã xác thực trống.");
                return ServiceResult<string>.Error("Thông tin không hợp lệ.");
            }

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Không tìm thấy người dùng với email: {Email}", request.Email);
                return ServiceResult<string>.Error("Tài khoản không tồn tại.");
            }

            var userSecurity = await _userSecurityRepository.GetByIdAsync(user.UserId);
            if (userSecurity == null)
            {
                _logger.LogError("Không tìm thấy bảo mật cho UserId: {UserId}", user.UserId);
                return ServiceResult<string>.Error("Không thể xác thực.");
            }

            if (userSecurity.TwoFactorSecret != request.SecretCode)
            {
                _logger.LogWarning("Mã xác thực 2FA không đúng cho email: {Email}", request.Email);
                return ServiceResult<string>.Error("Mã xác thực không chính xác.");
            }

            var log = new Log
            {
                LogId = Guid.NewGuid().ToString(),
                UserId = user.UserId,
                Action = $"Xác thực 2FA thành công cho email {request.Email}",
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                userSecurity.TwoFactorEnabled = true;
                userSecurity.TwoFactorSecret = null; // clear mã sau khi xác thực
                await _userSecurityRepository.UpdateAsync(userSecurity);
                await _logRepository.AddAsync(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật trạng thái xác thực 2FA cho {Email}", request.Email);
                return ServiceResult<string>.Error("Xác thực thất bại.");
            }

            return ServiceResult<string>.Success("Xác thực thành công.");
        }
    }
}
