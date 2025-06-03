using DPTS.Applications.Interfaces;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Data;
using MailKit.Net.Smtp;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using MimeKit;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DPTS.Applications.Implements
{
    /// <summary>
    /// Service xử lý xác thực người dùng: đăng ký, đăng nhập, 2FA, gửi email, sinh JWT.
    /// </summary>
    public class AuthService : IAuthService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<AuthService> _logger;
        private readonly IConfiguration _configuration;

        public AuthService(ApplicationDbContext context, ILogger<AuthService> logger, IConfiguration configuration)
        {
            _context = context;
            _logger = logger;
            _configuration = configuration;
        }

        /// <summary>
        /// Đăng ký người dùng mới và gửi mã xác thực 2FA qua email.
        /// </summary>
        public async Task<ServiceResult<string>> RegisterAsync(string email, string password, string passwordComfirmed, bool isBuyer = true, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Registering new user with email: {Email}", email);

            if (password != passwordComfirmed)
                return ServiceResult<string>.Error("Mật khẩu xác nhận không khớp.");

            if (await _context.Users.AnyAsync(u => u.Email == email, cancellationToken))
                return ServiceResult<string>.Error("Email đã được sử dụng.");

            var roleName = isBuyer ? "Buyer" : "Seller";
            var role = await _context.Roles.FirstOrDefaultAsync(x => x.RoleName == roleName, cancellationToken);
            if (role == null)
                return ServiceResult<string>.Error("Không tìm thấy vai trò phù hợp.");

            var newUser = new User
            {
                UserId = Guid.NewGuid().ToString(),
                FullName = "Node",
                Email = email,
                Address = null!,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
                RoleId = role.RoleId,
                CreatedAt = DateTime.UtcNow,
                TwoFactorSecret = new Random().Next(1000000, 9999999).ToString(),
                TwoFactorEnabled = false
            };

            var log = new Log
            {
                LogId = Guid.NewGuid().ToString(),
                Action = $"{newUser.FullName} vừa đăng ký tài khoản mới.",
                CreatedAt = DateTime.UtcNow,
                UserId = newUser.UserId
            };

            try
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

                await _context.Users.AddAsync(newUser, cancellationToken);
                await _context.Logs.AddAsync(log, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                if (!SendMail(email, newUser.TwoFactorSecret))
                    return ServiceResult<string>.Error("Gửi email xác nhận thất bại.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng ký người dùng.");
                return ServiceResult<string>.Error("Đã xảy ra lỗi khi đăng ký.");
            }

            return ServiceResult<string>.Success("Đăng ký thành công. Vui lòng kiểm tra email để xác thực.");
        }

        /// <summary>
        /// Xác thực mã 2FA được gửi qua email khi đăng ký.
        /// </summary>
        public async Task<ServiceResult<string>> Auth2FAAsync(string email, string twoFactorSecret, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Xác thực 2FA cho email: {Email}", email);

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email, cancellationToken);
            if (user == null)
                return ServiceResult<string>.Error("Người dùng không tồn tại.");

            if (user.TwoFactorSecret != twoFactorSecret)
                return ServiceResult<string>.Error("Mã xác thực không đúng.");

            var log = new Log
            {
                LogId = Guid.NewGuid().ToString(),
                Action = $"{user.FullName} vừa xác thực 2FA.",
                CreatedAt = DateTime.UtcNow,
                UserId = user.UserId
            };

            try
            {
                await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

                user.TwoFactorEnabled = true;
                _context.Entry(user).State = EntityState.Modified;

                await _context.Logs.AddAsync(log, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi xác thực 2FA.");
                return ServiceResult<string>.Error("Xác thực thất bại.");
            }

            return ServiceResult<string>.Success("Xác thực 2FA thành công.");
        }

        /// <summary>
        /// Đăng nhập người dùng, trả về JWT nếu thành công.
        /// </summary>
        public async Task<ServiceResult<string>> LoginAsync(string email, string password, CancellationToken cancellationToken = default)
        {
            var user = await _context.Users
                                     .Include(x => x.Role)
                                     .FirstOrDefaultAsync(x => x.Email == email, cancellationToken);

            if (user == null || !BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
                return ServiceResult<string>.Error("Email hoặc mật khẩu không đúng.");

            var log = new Log
            {
                LogId = Guid.NewGuid().ToString(),
                Action = $"{user.FullName} đã đăng nhập.",
                CreatedAt = DateTime.UtcNow,
                UserId = user.UserId
            };

            try
            {
                var token = GenerateToken(user);

                await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);

                await _context.Logs.AddAsync(log, cancellationToken);
                await _context.SaveChangesAsync(cancellationToken);

                await transaction.CommitAsync(cancellationToken);

                return ServiceResult<string>.Success(token);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi đăng nhập.");
                return ServiceResult<string>.Error("Đăng nhập thất bại.");
            }
        }

        /// <summary>
        /// Tạo JWT token chứa thông tin người dùng.
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
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        /// <summary>
        /// Gửi email xác thực chứa mã 2FA.
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
                _logger.LogError(ex, "Gửi email thất bại đến địa chỉ: {ToAddress}", toAddress);
                return false;
            }
        }
    }
}
