using DPTS.Applications.NoDistinctionOfRoles.auths.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MailKit.Net.Smtp;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;
using System.Text.RegularExpressions;

namespace DPTS.Applications.NoDistinctionOfRoles.auths.Handles
{
    public class SendRequest2FAHandle : IRequestHandler<SendRequest2FAQuery, ServiceResult<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<SendRequest2FAHandle> _logger;
        private readonly IUserSecurityRepository _userSecurityRepository;
        private readonly ILogRepository _logRepository;
        private readonly IConfiguration _configuration;

        public SendRequest2FAHandle(
            IUserRepository userRepository,
            ILogger<SendRequest2FAHandle> logger,
            IUserSecurityRepository userSecurityRepository,
            ILogRepository logRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _logger = logger;
            _userSecurityRepository = userSecurityRepository;
            _logRepository = logRepository;
            _configuration = configuration;
        }

        public async Task<ServiceResult<string>> Handle(SendRequest2FAQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || !IsValidEmail(request.Email))
            {
                _logger.LogWarning("Email không hợp lệ: {Email}", request.Email);
                return ServiceResult<string>.Error("Email không hợp lệ.");
            }

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("Không tìm thấy người dùng với email: {Email}", request.Email);
                return ServiceResult<string>.Error("Người dùng không tồn tại.");
            }

            var userSecurity = await _userSecurityRepository.GetByIdAsync(user.UserId);
            if (userSecurity == null)
            {
                _logger.LogError("Không tìm thấy thông tin bảo mật cho UserId: {UserId}", user.UserId);
                return ServiceResult<string>.Error("Không thể xử lý xác thực.");
            }

            var code2FA = Generate2FACode();
            userSecurity.TwoFactorSecret = code2FA;

            if (!SendMail(request.Email, code2FA))
            {
                _logger.LogError("Không gửi được mã 2FA tới email: {Email}", request.Email);
                return ServiceResult<string>.Error("Gửi email thất bại.");
            }

            var log = new Log
            {
                LogId = Guid.NewGuid().ToString(),
                Action = $"Yêu cầu mã 2FA gửi đến email {request.Email}",
                CreatedAt = DateTime.UtcNow,
                UserId = user.UserId
            };

            try
            {
                await _userSecurityRepository.UpdateAsync(userSecurity);
                await _logRepository.AddAsync(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi cập nhật 2FA hoặc ghi log.");
                return ServiceResult<string>.Error("Không thể lưu thông tin xác thực.");
            }

            return ServiceResult<string>.Success("Mã xác thực đã được gửi.");
        }

        private static string Generate2FACode()
        {
            var random = new Random();
            return random.Next(100000, 999999).ToString();
        }

        private static bool IsValidEmail(string email)
        {
            return Regex.IsMatch(email, @"^[^@\s]+@[^@\s]+\.[^@\s]+$");
        }

        private bool SendMail(string toAddress, string twoFactorSecret)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_configuration["Mail:From"]!));
                email.To.Add(MailboxAddress.Parse(toAddress));
                email.Subject = "Xác thực đăng nhập - DPTS";

                email.Body = new TextPart("plain")
                {
                    Text = $"Chào bạn,\n\nMã xác thực hai bước (2FA) của bạn là: {twoFactorSecret}\n\nMã này có hiệu lực trong 5 phút.\n\nTrân trọng,\nDPTS Team"
                };

                using var smtp = new SmtpClient();
                smtp.Connect(
                    _configuration["Mail:SmtpHost"],
                    int.Parse(_configuration["Mail:SmtpPort"]!),
                    MailKit.Security.SecureSocketOptions.StartTls);

                smtp.Authenticate(_configuration["Mail:Username"], _configuration["Mail:Password"]);
                smtp.Send(email);
                smtp.Disconnect(true);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi gửi email xác thực đến {Email}", toAddress);
                return false;
            }
        }
    }
}
