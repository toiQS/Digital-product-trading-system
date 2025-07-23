using DPTS.Applications.Auth.Dtos;
using DPTS.Applications.Auth.Queries;
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

namespace DPTS.Applications.Auth.Handles
{
    public class LoginHandle : IRequestHandler<LoginQuery, ServiceResult<LoginDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserSecurityRepository _userSecurityRepository;
        private readonly ILogRepository _logRepository;
        private readonly IRoleRepository _roleRepository;
        private readonly IConfiguration _configuration;
        private readonly ILogger<LoginHandle> _logger;

        public LoginHandle(
            IUserRepository userRepository,
            IUserSecurityRepository userSecurityRepository,
            ILogRepository logRepository,
            IRoleRepository roleRepository,
            IConfiguration configuration,
            ILogger<LoginHandle> logger)
        {
            _userRepository = userRepository;
            _userSecurityRepository = userSecurityRepository;
            _logRepository = logRepository;
            _roleRepository = roleRepository;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<ServiceResult<LoginDto>> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Login attempt for email: {Email}", request.Email);

            var user = await _userRepository.GetByEmailAsync(request.Email);
            if (user == null)
            {
                _logger.LogWarning("User not found for email: {Email}", request.Email);
                return ServiceResult<LoginDto>.Error("Tài khoản hoặc mật khẩu không chính xác.");
            }
            if (user.IsActive == false)
            {
                _logger.LogWarning("User is not avalible");
                return ServiceResult<LoginDto>.Error("Tài khoản đã bị vô hiệu");
            }
            var userSecurity = await _userSecurityRepository.GetByUserIdAsync(user.UserId);
            if (userSecurity == null)
            {
                _logger.LogWarning("No security data for userId: {UserId}", user.UserId);
                return ServiceResult<LoginDto>.Error("Không thể xác thực người dùng.");
            }

            if (!BCrypt.Net.BCrypt.Verify(request.Password, userSecurity.PasswordHash))
            {
                _logger.LogWarning("Password mismatch for userId: {UserId}", user.UserId);
                return ServiceResult<LoginDto>.Error("Tài khoản hoặc mật khẩu không chính xác.");
            }

            var role = await _roleRepository.GetByIdAsync(user.RoleId);
            if (role == null)
            {
                _logger.LogWarning("Role not found for roleId: {RoleId}", user.RoleId);
                return ServiceResult<LoginDto>.Error("Quyền không hợp lệ.");
            }

            var log = new Log
            {
                LogId = Guid.NewGuid().ToString(),
                Action = "Login",
                UserId = user.UserId,
                CreatedAt = DateTime.UtcNow
            };

            try
            {
                await _logRepository.AddAsync(log);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to save login log for userId: {UserId}", user.UserId);
            }

            var token = GenerateToken(user.UserId, user.Email, role.RoleName);

            return ServiceResult<LoginDto>.Success(new LoginDto
            {
                UserId = user.UserId,
                FullName = user.Email, // TODO: Replace with FullName when available
                Role = role.RoleName,
                IsTwoFactorRequired = userSecurity.TwoFactorEnabled,
                IsEmailVerified = userSecurity.EmailVerified,
                Token = new JwtTokenDto
                {
                    AccessToken = token,
                    ExpiresAt = DateTime.UtcNow.AddMinutes(30),
                    RefreshToken = token
                }
            });
        }

        private string GenerateToken(string userId, string email, string role)
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, userId),
                new Claim(ClaimTypes.Email, email),
                new Claim(ClaimTypes.Role, role)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:SecretKey"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
