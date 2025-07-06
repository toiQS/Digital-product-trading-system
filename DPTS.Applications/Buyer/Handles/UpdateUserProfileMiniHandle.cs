using DPTS.Applications.Buyer.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles;

/// <summary>
/// Handler xử lý cập nhật thông tin hồ sơ rút gọn (mini) của người dùng.
/// </summary>
public class UpdateUserProfileMiniHandle : IRequestHandler<UpdateUserProfileMiniQuery, ServiceResult<string>>
{
    private readonly ILogger<UpdateUserProfileMiniHandle> _logger;
    private readonly IUserRepository _userRepository;
    private readonly ILogRepository _logRepository;
    private readonly IUserSecurityRepository _userSecurityRepository;
    private readonly IUserProfileRepository _userProfileRepository;

    public UpdateUserProfileMiniHandle(
        ILogger<UpdateUserProfileMiniHandle> logger,
        IUserRepository userRepository,
        ILogRepository logRepository,
        IUserSecurityRepository userSecurityRepository,
        IUserProfileRepository userProfileRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _logRepository = logRepository;
        _userSecurityRepository = userSecurityRepository;
        _userProfileRepository = userProfileRepository;
    }

    public async Task<ServiceResult<string>> Handle(UpdateUserProfileMiniQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling mini profile update for user: {UserId}", request.UserId);

        // 1. Lấy thông tin người dùng
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            _logger.LogError("User not found: {UserId}", request.UserId);
            return ServiceResult<string>.Error("Không tìm thấy người dùng.");
        }

        // 2. Lấy thông tin hồ sơ người dùng
        var profile = await _userProfileRepository.GetByUserIdAsync(user.UserId);
        if (profile == null)
        {
            _logger.LogError("User profile not found: {UserId}", request.UserId);
            return ServiceResult<string>.Error("Không tìm thấy hồ sơ người dùng.");
        }

        // 3. Lấy thông tin bảo mật người dùng
        var security = await _userSecurityRepository.GetByUserIdAsync(user.UserId);
        if (security == null)
        {
            _logger.LogError("User security not found: {UserId}", request.UserId);
            return ServiceResult<string>.Error("Không tìm thấy thông tin bảo mật.");
        }

        try
        {
            // 4. Cập nhật thông tin người dùng và hồ sơ
            if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
            {
                user.Email = request.Email;
                security.EmailVerified = false;
            }

            profile.FullName = request.FullName ?? profile.FullName;
            profile.ImageUrl = request.ImageUser ?? profile.ImageUrl;

            // 5. Cập nhật cơ sở dữ liệu
            await _userRepository.UpdateAsync(user);
            await _userProfileRepository.UpdateAsync(profile);
            await _userSecurityRepository.UpdateAsync(security);

            return ServiceResult<string>.Success("Cập nhật hồ sơ thành công.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi cập nhật hồ sơ mini cho người dùng: {UserId}", request.UserId);
            return ServiceResult<string>.Error("Đã xảy ra lỗi khi cập nhật hồ sơ.");
        }
    }
}
