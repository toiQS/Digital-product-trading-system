using DPTS.Applications.Buyer.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles;

/// <summary>
/// Xử lý cập nhật thông tin hồ sơ người dùng (User & Profile).
/// </summary>
public class UpdateUserProfileHandler : IRequestHandler<UpdateUserProfileQuery, ServiceResult<string>>
{
    private readonly ILogger<UpdateUserProfileHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUserSecurityRepository _userSecurityRepository;

    public UpdateUserProfileHandler(
        ILogger<UpdateUserProfileHandler> logger,
        IUserRepository userRepository,
        ILogRepository logRepository,
        IUserProfileRepository userProfileRepository,
        IUserSecurityRepository userSecurityRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _logRepository = logRepository;
        _userProfileRepository = userProfileRepository;
        _userSecurityRepository = userSecurityRepository;
    }

    public async Task<ServiceResult<string>> Handle(UpdateUserProfileQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling profile update for user: {UserId}", request.UserId);

        // 1. Lấy thông tin user
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            _logger.LogWarning("User not found: {UserId}", request.UserId);
            return ServiceResult<string>.Error("Không tìm thấy người dùng.");
        }

        // 2. Lấy thông tin hồ sơ
        var profile = await _userProfileRepository.GetByUserIdAsync(user.UserId);
        if (profile == null)
        {
            _logger.LogError("User profile not found: {UserId}", request.UserId);
            return ServiceResult<string>.Error("Không tìm thấy hồ sơ người dùng.");
        }

        // 3. Lấy thông tin bảo mật
        var security = await _userSecurityRepository.GetByUserIdAsync(user.UserId);
        if (security == null)
        {
            _logger.LogError("User security not found: {UserId}", request.UserId);
            return ServiceResult<string>.Error("Không tìm thấy thông tin bảo mật.");
        }

        // 4. Khởi tạo address nếu chưa có
        profile.Address ??= new Address();

        try
        {
            // 5. Cập nhật thông tin hồ sơ người dùng
            profile.FullName = request.FullName ?? profile.FullName;
            profile.Phone = request.PhoneNumber ?? profile.Phone;
            profile.Address.Street = request.Street ?? profile.Address.Street;
            profile.Address.City = request.City ?? profile.Address.City;
            profile.Address.District = request.District ?? profile.Address.District;
            profile.Address.Country = request.Country ?? profile.Address.Country;
            profile.Address.PostalCode = request.PostalCode ?? profile.Address.PostalCode;

            // 6. Cập nhật email và trạng thái xác minh
            if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
            {
                user.Email = request.Email;
                security.EmailVerified = false;
            }

            user.UpdatedAt = DateTime.UtcNow;

            // 7. Ghi log thao tác
            var log = new Log
            {
                LogId = Guid.NewGuid().ToString(),
                UserId = user.UserId,
                Action = "UpdateUserProfile",
                TargetId = user.UserId,
                TargetType = "User",
                CreatedAt = DateTime.UtcNow
            };

            // 8. Lưu thay đổi
            await _userRepository.UpdateAsync(user);
            await _userProfileRepository.UpdateAsync(profile);
            await _userSecurityRepository.UpdateAsync(security);
            await _logRepository.AddAsync(log);

            return ServiceResult<string>.Success("Cập nhật hồ sơ thành công.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to update user profile for user: {UserId}", request.UserId);
            return ServiceResult<string>.Error("Đã xảy ra lỗi khi cập nhật hồ sơ.");
        }
    }
}
