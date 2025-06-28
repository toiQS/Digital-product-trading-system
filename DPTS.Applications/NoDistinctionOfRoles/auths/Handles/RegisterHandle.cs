using DPTS.Applications.NoDistinctionOfRoles.auths.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.NoDistinctionOfRoles.auths.Handles
{
    public class RegisterHandle : IRequestHandler<RegisterQuery, ServiceResult<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserSecurityRepository _userSecurityRepository;
        private readonly IUserProfileRepository _profileRepository;
        private readonly ILogRepository _logRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly ILogger<RegisterHandle> _logger;

        public RegisterHandle(
            IUserRepository userRepository,
            IUserSecurityRepository userSecurityRepository,
            IUserProfileRepository profileRepository,
            ILogRepository logRepository,
            IStoreRepository storeRepository,
            ILogger<RegisterHandle> logger)
        {
            _userRepository = userRepository;
            _userSecurityRepository = userSecurityRepository;
            _profileRepository = profileRepository;
            _logRepository = logRepository;
            _storeRepository = storeRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<string>> Handle(RegisterQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Bắt đầu xử lý đăng ký tài khoản cho email: {Email}", request.Email);

            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                _logger.LogWarning("Thiếu thông tin email hoặc password.");
                return ServiceResult<string>.Error("Thông tin đăng ký không hợp lệ.");
            }

            var existingUser = await _userRepository.GetByEmailAsync(request.Email);
            if (existingUser != null)
            {
                _logger.LogWarning("Email đã được sử dụng: {Email}", request.Email);
                return ServiceResult<string>.Error("Email đã tồn tại trong hệ thống.");
            }

            var userId = Guid.NewGuid().ToString();
            var now = DateTime.UtcNow;

            var user = new User
            {
                UserId = userId,
                Email = request.Email,
                RoleId = request.IsBuyer ? "Buyer" : "Seller",
                CreatedAt = now,
                UpdatedAt = now,
                IsActive = true
            };

            var userSecurity = new UserSecurity
            {
                UserId = userId,
                PasswordHash = PasswordHasher.Hash(request.Password),
                TwoFactorEnabled = false,
                LockoutUntil = now,
                FailedLoginAttempts = 0
            };

            var userProfile = new UserProfile
            {
                UserId = userId,
                Bio = string.Empty,
                BirthDate = default,
            };

            var log = new Log
            {
                LogId = Guid.NewGuid().ToString(),
                Action = "Người dùng đăng ký tài khoản",
                CreatedAt = now,
                UserId = userId
            };

            try
            {
                await _userRepository.AddAsync(user);
                await _userSecurityRepository.AddAsync(userSecurity);
                await _profileRepository.AddAsync(userProfile);

                if (!request.IsBuyer)
                {
                    var store = new Store
                    {
                        StoreId = Guid.NewGuid().ToString(),
                        UserId = userId,
                        StoreName = $"Store of {request.Email}",
                        CreateAt = now,
                        Status = StoreStatus.Active
                    };

                    await _storeRepository.AddAsync(store);
                }

                await _logRepository.AddAsync(log);

                _logger.LogInformation("Đăng ký tài khoản thành công cho email: {Email}", request.Email);
                return ServiceResult<string>.Success("Đăng ký tài khoản thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong quá trình đăng ký tài khoản.");
                return ServiceResult<string>.Error("Có lỗi xảy ra khi tạo tài khoản.");
            }
        }
    }
}
