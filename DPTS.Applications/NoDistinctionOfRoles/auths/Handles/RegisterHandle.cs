using DPTS.Applications.NoDistinctionOfRoles.auths.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MailKit.Net.Smtp;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

namespace DPTS.Applications.NoDistinctionOfRoles.auths.Handles
{
    public class RegisterHandle : IRequestHandler<RegisterQuery, ServiceResult<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<RegisterHandle> _logger;
        private readonly ILogRepository _logRepository;
        private readonly IConfiguration _configuration;

        public RegisterHandle(
            IUserRepository userRepository,
            ILogger<RegisterHandle> logger,
            ILogRepository logRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _logger = logger;
            _logRepository = logRepository;
            _configuration = configuration;
        }

        public async Task<ServiceResult<string>> Handle(RegisterQuery query, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Register request received for email: {Email}", query.Email);

            try
            {
                var userId = Guid.NewGuid().ToString();
                var twoFactorSecret = Guid.NewGuid().ToString();

                var user = new User
                {
                    UserId = userId,
                    Email = query.Email,
                    PasswordHash = query.Password, 
                    Username = query.Email.Split('@')[0],
                    RoleId = query.IsBuyer ? "Buyer" : "Seller",
                    TwoFactorEnabled = false,
                    TwoFactorSecret = twoFactorSecret
                };

                var roleText = query.IsBuyer ? "người mua" : "người bán";
                var log = new Log
                {
                    LogId = Guid.NewGuid().ToString(),
                    CreatedAt = DateTime.UtcNow,
                    Action = $"Có một {roleText} đăng ký vào hệ thống."
                };

                var isMailSent = SendMail(user.Email, user.TwoFactorSecret);
                if (!isMailSent)
                {
                    _logger.LogWarning("Failed to send registration email to {Email}", user.Email);
                    return ServiceResult<string>.Error("Không thể gửi email xác nhận.");
                }

                await _userRepository.AddAsync(user);
                await _logRepository.AddAsync(log);

                return ServiceResult<string>.Success("Đăng ký thành công. Vui lòng kiểm tra email để lấy mã 2FA.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error occurred during registration for email: {Email}", query.Email);
                return ServiceResult<string>.Error("Đăng ký thất bại do lỗi hệ thống.");
            }
        }

        /// <summary>
        /// Gửi email xác nhận đăng ký với mã 2FA.
        /// </summary>
        private bool SendMail(string toAddress, string twoFactorSecret)
        {
            try
            {
                var email = new MimeMessage();
                email.From.Add(MailboxAddress.Parse(_configuration["Mail:From"]));
                email.To.Add(MailboxAddress.Parse(toAddress));
                email.Subject = "Welcome to DPTS!";
                email.Body = new TextPart("plain")
                {
                    Text = $"Cảm ơn bạn đã đăng ký tại DPTS.\nMã xác thực hai bước (2FA) của bạn là: {twoFactorSecret}"
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
                _logger.LogError(ex, "Failed to send email to {Email}", toAddress);
                return false;
            }
        }
    }
}
