using DPTS.Applications.NoDistinctionOfRoles.auths.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DPTS.Applications.NoDistinctionOfRoles.auths.Handles
{
    public class _2FAHandle : IRequestHandler<_2FAQuery, ServiceResult<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogRepository _logRepository;
        private readonly ILogger<_2FAHandle> _logger;

        public _2FAHandle(
            IUserRepository userRepository,
            ILogRepository logRepository,
            ILogger<_2FAHandle> logger)
        {
            _userRepository = userRepository;
            _logRepository = logRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<string>> Handle(_2FAQuery query, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("2FA verification attempt for email: {Email}", query.Email);

            try
            {
                var user = await _userRepository.GetByEmailAsync(query.Email);
                if (user == null)
                {
                    _logger.LogWarning("2FA failed: User not found - {Email}", query.Email);
                    return ServiceResult<string>.Error("Tài khoản không tồn tại.");
                }

                if (user.TwoFactorSecret != query.SecretCode)
                {
                    _logger.LogWarning("2FA failed: Wrong secret code for user {Email}", query.Email);
                    return ServiceResult<string>.Error("Mã xác thực không hợp lệ.");
                }

                user.TwoFactorEnabled = true;

                var log = new Log
                {
                    LogId = Guid.NewGuid().ToString(),
                    UserId = user.UserId,
                    Action = $"{user.FullName} đã bật xác thực 2 lớp thành công.",
                    CreatedAt = DateTime.UtcNow
                };

                await _userRepository.UpdateAsync(user);
                await _logRepository.AddAsync(log);

                _logger.LogInformation("2FA successfully enabled for user: {Email}", query.Email);
                return ServiceResult<string>.Success("Xác thực hai lớp đã được kích hoạt.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during 2FA verification for user: {Email}", query.Email);
                return ServiceResult<string>.Error("Xác thực hai lớp thất bại do lỗi hệ thống.");
            }
        }
    }
}
