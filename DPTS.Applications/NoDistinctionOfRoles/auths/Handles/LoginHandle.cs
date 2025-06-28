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
        private readonly ILogger<LoginHandle> _logger;
        private readonly ILogRepository _logRepository;
        private readonly IConfiguration _configuration;

        public LoginHandle(
            IUserRepository userRepository,
            ILogger<LoginHandle> logger,
            ILogRepository logRepository,
            IConfiguration configuration)
        {
            _userRepository = userRepository;
            _logger = logger;
            _logRepository = logRepository;
            _configuration = configuration;
        }

        public async Task<ServiceResult<LoginDto>> Handle(LoginQuery request, CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogWarning("Thiếu email hoặc mật khẩu.");
                return ServiceResult<LoginDto>.Error("Thông tin đăng nhập không hợp lệ.");
            }

            try
            {
                var user = await _userRepository.GetByEmailAsync(request.Email, includeSecurity: true);

                if (user == null || user.Security == null)
                {
                    _logger.LogWarning("Người dùng không tồn tại hoặc thiếu thông tin bảo mật: {Email}", request.Email);
                    return ServiceResult<LoginDto>.Error("Email hoặc mật khẩu không đúng.");
                }

                
                if (!PasswordHasher.Verify(request.Password, user.Security.PasswordHash))
                {
                    _logger.LogWarning("Mật khẩu không hợp lệ cho email: {Email}", request.Email);
                    return ServiceResult<LoginDto>.Error("Email hoặc mật khẩu không đúng.");
                }

                if (!user.IsActive)
                {
                    _logger.LogWarning("Tài khoản bị khóa: {UserId}", user.UserId);
                    return ServiceResult<LoginDto>.Error("Tài khoản của bạn đang bị khóa.");
                }

                var token = GenerateToken(user);

                await _logRepository.AddAsync(new Log
                {
                    LogId = Guid.NewGuid().ToString(),
                    UserId = user.UserId,
                    CreatedAt = DateTime.UtcNow,
                    Action = "Đăng nhập thành công"
                });

                var result = new LoginDto
                {
                    Expiry = DateTime.UtcNow.AddMinutes(30),
                    Token = token
                };

                _logger.LogInformation("Đăng nhập thành công cho userId: {UserId}", user.UserId);

                return ServiceResult<LoginDto>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi hệ thống khi xử lý đăng nhập.");
                return ServiceResult<LoginDto>.Error("Có lỗi xảy ra trong quá trình đăng nhập.");
            }
        }

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
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(jwtToken);
        }
    }
}
