using DPTS.Applications.NoDistinctionOfRoles.auths.Dtos;
using DPTS.Applications.NoDistinctionOfRoles.auths.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace DPTS.Applications.NoDistinctionOfRoles.auths.Handles
{
    public class LoginHandle : IRequestHandler<LoginQuery, ServiceResult<LoginDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogRepository _logRepository;
        private readonly ILogger<LoginHandle> _logger;
        private readonly IConfiguration _configuration;

        public LoginHandle(
            IUserRepository userRepository,
            ILogRepository logRepository,
            ILogger<LoginHandle> logger,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _logRepository = logRepository;
            _logger = logger;
            _configuration = configuration;
        }

        public async Task<ServiceResult<LoginDto>> Handle(LoginQuery query, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Login attempt for email: {Email}", query.Email);

            try
            {
                var user = await _userRepository.GetByEmailAsync(query.Email);

                if (user == null)
                {
                    _logger.LogWarning("Login failed: Email not found - {Email}", query.Email);
                    return ServiceResult<LoginDto>.Error("Tài khoản không tồn tại.");
                }

                if (user.PasswordHash != query.Password) // Cần hash password để so sánh nếu dùng thực tế
                {
                    _logger.LogWarning("Login failed: Incorrect password for user - {Email}", query.Email);
                    return ServiceResult<LoginDto>.Error("Sai mật khẩu.");
                }

                var log = new Log
                {
                    LogId = Guid.NewGuid().ToString(),
                    Action = $"{user.Username} vừa đăng nhập",
                    CreatedAt = DateTime.UtcNow,
                    UserId = user.UserId
                };
                await _logRepository.AddAsync(log);

                var token = GenerateToken(user);
                var result = new LoginDto
                {
                    Token = token,
                    Expiry = DateTime.UtcNow.AddMinutes(30)
                };

                _logger.LogInformation("Login successful for user: {Email}", query.Email);
                return ServiceResult<LoginDto>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during login for user: {Email}", query.Email);
                return ServiceResult<LoginDto>.Error("Đăng nhập thất bại do lỗi hệ thống.");
            }
        }

        /// <summary>
        /// Tạo JWT Token chứa thông tin cơ bản của người dùng.
        /// </summary>
        private string GenerateToken(User user)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.UserId),
                new Claim(ClaimTypes.Email, user.Email),
                new Claim(ClaimTypes.Role, user.Role?.RoleName ?? "Unknown")
            };

            var key = Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!);
            var credentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256);

            var jwtToken = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: credentials);

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
    }
}
