using DPTS.Applications.Auth.Dtos;
using DPTS.Applications.Auth.Handles;
using DPTS.Applications.Auth.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MailKit.Net.Smtp;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using MimeKit;

public class RegisterHandle : IRequestHandler<RegisterQuery, ServiceResult<RegisterDto>>
{
    private readonly ILogRepository _logRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUserSecurityRepository _userSecurityRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IConfiguration _configuration;
    private readonly ILogger<RegisterHandle> _logger;

    public RegisterHandle(
        ILogRepository logRepository,
        IUserRepository userRepository,
        IUserSecurityRepository userSecurityRepository,
        IUserProfileRepository userProfileRepository,
        IConfiguration configuration,
        ILogger<RegisterHandle> logger)
    {
        _logRepository = logRepository;
        _userRepository = userRepository;
        _userSecurityRepository = userSecurityRepository;
        _userProfileRepository = userProfileRepository;
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<ServiceResult<RegisterDto>> Handle(RegisterQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Register attempt started for email: {Email}", request.Email);

        if (request.Password != request.PasswordComfirmed)
        {
            _logger.LogWarning("Password confirmation mismatch for email: {Email}", request.Email);
            return ServiceResult<RegisterDto>.Error("Mật khẩu xác nhận không khớp.");
        }

        var existingUser = await _userRepository.GetByEmailAsync(request.Email);
        if (existingUser != null)
        {
            _logger.LogWarning("Email already exists: {Email}", request.Email);
            return ServiceResult<RegisterDto>.Error("Email đã được sử dụng.");
        }

        var userId = Guid.NewGuid().ToString();
        var isBuyer = request.IsBuyer;
        var role = isBuyer ? "Buyer" : "Seller";

        var user = new User
        {
            UserId = userId,
            Email = request.Email,
            Username = "",
            RoleId = role,
            CreatedAt = DateTime.UtcNow,
            UpdatedAt = DateTime.UtcNow,
            IsActive = true,
            Store = isBuyer ? null : new Store
            {
                StoreId = Guid.NewGuid().ToString(),
                UserId = userId,
                StoreName = $"Store of {request.Email}",
                CreateAt = DateTime.UtcNow,
                Status = StoreStatus.Active
            }
        };

        var profile = new UserProfile
        {
            UserId = userId
        };

        var security = new UserSecurity
        {
            UserId = userId,
            EmailVerified = false,
            TwoFactorEnabled = request.IsEnabled2FA,
            FailedLoginAttempts = 0,
            LockoutUntil = DateTime.UtcNow,
            PasswordHash = PasswordHasher.Hash(request.Password)
        };

        if (request.IsEnabled2FA)
        {
            var random = new Random();
            security.TwoFactorSecret = random.Next(100000, 999999).ToString();

            if (!SendMail(request.Email, security.TwoFactorSecret))
            {
                _logger.LogError("Failed to send 2FA code to email: {Email}", request.Email);
                return ServiceResult<RegisterDto>.Error("Không thể gửi mã xác thực hai bước.");
            }
        }

        var log = new Log
        {
            LogId = Guid.NewGuid().ToString(),
            UserId = userId,
            Action = "Register",
            CreatedAt = DateTime.UtcNow,
            TargetId = userId,
            TargetType = "User",
            UserAgent = "", // Có thể gán từ controller
            IpAddress = "", // Có thể gán từ controller
            UserType = role
        };

        try
        {
            await _userRepository.AddAsync(user);
            await _userSecurityRepository.AddAsync(security);
            await _userProfileRepository.AddAsync(profile);
            await _logRepository.AddAsync(log);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Exception occurred while registering user: {Email}", request.Email);
            return ServiceResult<RegisterDto>.Error("Đã xảy ra lỗi. Đăng ký thất bại.");
        }

        _logger.LogInformation("User registered successfully. Email: {Email}, UserId: {UserId}", request.Email, userId);
        return ServiceResult<RegisterDto>.Success(new RegisterDto
        {
            UserId = userId,
            IsEmailConfirmationRequired = request.IsEnabled2FA
        });
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
                Text = $"Chào mừng bạn đến với DPTS.\nMã xác thực 2FA của bạn là: {twoFactorSecret}"
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
            _logger.LogError(ex, "Failed to send email to {Email}", toAddress);
            return false;
        }
    }
}
