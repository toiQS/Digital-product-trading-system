using DPTS.Applications.Auth.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MailKit.Net.Smtp;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

public class Enable2FAHandle : IRequestHandler<Enable2FAQuery, ServiceResult<string>>
{
    private readonly IUserRepository _userRepository;
    private readonly IUserSecurityRepository _userSecurityRepository;
    private readonly ILogRepository _logRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<Enable2FAHandle> _logger;

    public Enable2FAHandle(
        IUserRepository userRepository,
        IUserSecurityRepository userSecurityRepository,
        ILogRepository logRepository,
        IConfiguration configuration,
        ILogger<Enable2FAHandle> logger)
    {
        _userRepository = userRepository;
        _userSecurityRepository = userSecurityRepository;
        _logRepository = logRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ServiceResult<string>> Handle(Enable2FAQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Begin enabling 2FA for email: {Email}", request.Email);

        var user = await _userRepository.GetByEmailAsync(request.Email);
        if (user == null)
        {
            _logger.LogWarning("User not found with email: {Email}", request.Email);
            return ServiceResult<string>.Error("Không tìm thấy người dùng.");
        }

        var security = await _userSecurityRepository.GetByUserIdAsync(user.UserId);
        if (security == null)
        {
            _logger.LogWarning("UserSecurity not found for UserId: {UserId}", user.UserId);
            return ServiceResult<string>.Error("Không thể kích hoạt 2FA. Tài khoản không hợp lệ.");
        }

        var random = new Random();
        security.TwoFactorSecret = random.Next(100000, 999999).ToString();
        security.TwoFactorEnabled = true;

        if (!SendMail(request.Email, security.TwoFactorSecret))
        {
            _logger.LogError("Failed to send 2FA code to email: {Email}", request.Email);
            return ServiceResult<string>.Error("Không thể gửi mã xác thực hai bước.");
        }

        var log = new Log
        {
            LogId = Guid.NewGuid().ToString(),
            UserId = user.UserId,
            Action = "Enable2FA",
            CreatedAt = DateTime.UtcNow,
            TargetId = user.UserId,
            TargetType = "User",
            IpAddress = "",
            UserAgent = "",
            UserType = "User"
        };

        try
        {
            await _userSecurityRepository.UpdateAsync(security);
            await _logRepository.AddAsync(log);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update 2FA or log action for userId: {UserId}", user.UserId);
            return ServiceResult<string>.Error("Lỗi khi lưu thông tin xác thực hai bước.");
        }

        _logger.LogInformation("2FA enabled successfully for userId: {UserId}", user.UserId);
        return ServiceResult<string>.Success("Đã bật xác thực hai bước thành công.");
    }

    private bool SendMail(string toAddress, string twoFactorSecret)
    {
        try
        {
            var email = new MimeMessage();
            email.From.Add(MailboxAddress.Parse(_configuration["Mail:From"]!));
            email.To.Add(MailboxAddress.Parse(toAddress));
            email.Subject = "Mã xác thực hai bước từ DPTS";
            email.Body = new TextPart("plain")
            {
                Text = $"Bạn đã yêu cầu bật xác thực hai bước (2FA).\nMã xác thực của bạn là: {twoFactorSecret}"
            };

            using var smtp = new SmtpClient();
            smtp.Connect(
                _configuration["Mail:SmtpHost"],
                int.Parse(_configuration["Mail:SmtpPort"]!),
                MailKit.Security.SecureSocketOptions.StartTls);

            smtp.Authenticate(_configuration["Mail:Username"], _configuration["Mail:Password"]);
            smtp.Send(email);
            smtp.Disconnect(true);

            _logger.LogInformation("2FA email sent to {Email}", toAddress);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception when sending 2FA email to {Email}", toAddress);
            return false;
        }
    }
}
