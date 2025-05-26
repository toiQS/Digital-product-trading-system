using DPTS.Applications.Dtos.auths;
using DPTS.Applications.Shareds;
using DPTS.Applications.Shareds.Interfaces;
using DPTS.Domains;
using MailKit.Net.Smtp;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DPTS.Applications.Implements
{
    public class AuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AuthService> _logger;
        private readonly IConfiguration _configuration;

        public AuthService(IUnitOfWork unitOfWork, ILogger<AuthService> logger, IConfiguration configuration)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Đăng ký tài khoản mới
        /// </summary>
        public async Task<ServiceResult<string>> RegisterAsync(RegisterModel model)
        {
            try
            {
                // Kiểm tra trùng email
                var existingUser = await _unitOfWork.Repository<User>().GetOneAsync(nameof(User.Email), model.Email);
                if (existingUser != null)
                    return ServiceResult<string>.Error("Email already exists.");

                // Tìm vai trò được yêu cầu
                var role = await _unitOfWork.Repository<Role>().GetOneAsync(nameof(Role.RoleName), model.RoleName);
                if (role == null)
                    return ServiceResult<string>.Error("Invalid role.");

                // Tạo người dùng mới
                var user = new User
                {
                    UserId = Guid.NewGuid().ToString(),
                    Email = model.Email,
                    Username = model.Email.Split('@')[0],
                    PasswordHash = BCrypt.Net.BCrypt.HashPassword(model.Password),
                    RoleId = role.RoleId,
                    TwoFactorEnabled = false,
                    TwoFactorSecret = Guid.NewGuid().ToString()
                };

                // Lưu người dùng vào DB
                await _unitOfWork.ExecuteTransactionAsync(async () =>
                    await _unitOfWork.Repository<User>().AddAsync(user));

                // Gửi email xác thực
                var sendMailResult = SendMail(user.Email, user.TwoFactorSecret);
                if (!sendMailResult)
                    return ServiceResult<string>.Error("Failed to send confirmation email.");

                return ServiceResult<string>.Success("User registered successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error while registering user {Email}", model.Email);
                return ServiceResult<string>.Error("Registration failed due to server error.");
            }
        }

        /// <summary>
        /// Xác thực 2 bước bằng mã đã gửi về email
        /// </summary>
        public async Task<ServiceResult<string>> Auth2FAAsync(Auth2FAModel model)
        {
            try
            {
                var user = await _unitOfWork.Repository<User>().GetOneAsync(nameof(User.Email), model.Email);
                if (user == null)
                    return ServiceResult<string>.Error("User not found.");

                if (user.TwoFactorSecret != model.TwoFactorSecret)
                    return ServiceResult<string>.Error("Invalid two-factor authentication code.");

                user.TwoFactorEnabled = true;
                user.UpdatedAt = DateTime.UtcNow;

                await _unitOfWork.ExecuteTransactionAsync(async () =>
                    await _unitOfWork.Repository<User>().UpdateAsync(user));

                return ServiceResult<string>.Success("Two-factor authentication enabled successfully.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "2FA failed for {Email}", model.Email);
                return ServiceResult<string>.Error("An error occurred during 2FA.");
            }
        }

        /// <summary>
        /// Đăng nhập người dùng
        /// </summary>
        public async Task<ServiceResult<LoginResult>> LoginAsync(LoginModel model)
        {
            try
            {
                var user = await _unitOfWork.Repository<User>().GetOneAsync(nameof(User.Email), model.Email);
                if (user == null)
                    return ServiceResult<LoginResult>.Error("User not found.");

                if (!BCrypt.Net.BCrypt.Verify(model.Password, user.PasswordHash))
                    return ServiceResult<LoginResult>.Error("Invalid password.");

                var loginResult = new LoginResult
                {
                    AccessToken = GenerateToken(user),
                    Expiry = DateTime.UtcNow.AddMinutes(30)
                };

                return ServiceResult<LoginResult>.Success(loginResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Login failed for {Email}", model.Email);
                return ServiceResult<LoginResult>.Error("An error occurred during login.");
            }
        }

        /// <summary>
        /// Sinh JWT Token cho người dùng
        /// </summary>
        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role.RoleName)
            };

            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Gửi email xác nhận đăng ký
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
                    Text = $"Thank you for registering with DPTS.\nYour 2FA code: {twoFactorSecret}"
                };

                using var smtp = new SmtpClient();
                smtp.Connect(_configuration["Mail:SmtpHost"], int.Parse(_configuration["Mail:SmtpPort"]!), MailKit.Security.SecureSocketOptions.StartTls);
                smtp.Authenticate(_configuration["Mail:Username"], _configuration["Mail:Password"]);
                smtp.Send(email);
                smtp.Disconnect(true);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {ToAddress}", toAddress);
                return false;
            }
        }
    }
}
