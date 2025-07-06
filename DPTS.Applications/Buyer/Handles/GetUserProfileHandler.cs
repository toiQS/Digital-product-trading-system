using DPTS.Applications.Buyer.Dtos;
using DPTS.Applications.Buyer.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles;

/// <summary>
/// Handler xử lý lấy thông tin hồ sơ chi tiết của người dùng.
/// </summary>
public class GetUserProfileHandler : IRequestHandler<GetUserProfileQuery, ServiceResult<UserProfileDto>>
{
    private readonly IUserRepository _userRepository;
    private readonly ILogger<GetUserProfileHandler> _logger;
    private readonly IUserProfileRepository _userProfileRepository;
    private readonly IUserSecurityRepository _userSecurityRepository;

    public GetUserProfileHandler(
        IUserRepository userRepository,
        ILogger<GetUserProfileHandler> logger,
        IUserProfileRepository userProfileRepository,
        IUserSecurityRepository userSecurityRepository)
    {
        _userRepository = userRepository;
        _logger = logger;
        _userProfileRepository = userProfileRepository;
        _userSecurityRepository = userSecurityRepository;
    }

    public async Task<ServiceResult<UserProfileDto>> Handle(GetUserProfileQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Fetching profile for user: {UserId}", request.UserId);

        // 1. Lấy thông tin user
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            _logger.LogError("User not found: {UserId}", request.UserId);
            return ServiceResult<UserProfileDto>.Error("Không tìm thấy người dùng.");
        }

        // 2. Lấy thông tin hồ sơ người dùng
        var profile = await _userProfileRepository.GetByUserIdAsync(user.UserId);
        if (profile == null)
        {
            _logger.LogError("User profile not found: {UserId}", request.UserId);
            return ServiceResult<UserProfileDto>.Error("Không tìm thấy hồ sơ người dùng.");
        }

        // 3. Lấy thông tin bảo mật
        var security = await _userSecurityRepository.GetByUserIdAsync(user.UserId);
        if (security == null)
        {
            _logger.LogError("User security not found: {UserId}", request.UserId);
            return ServiceResult<UserProfileDto>.Error("Không tìm thấy thông tin bảo mật.");
        }

        // 4. Mapping kết quả trả về
        var result = new UserProfileDto
        {
            UserId = user.UserId,
            FullName = profile.FullName ?? "Chưa có tên",
            PhoneNumber = profile.Phone ?? "Chưa có số điện thoại",
            Email = user.Email,
            IsEmailVerified = security.TwoFactorEnabled,

            City = profile.Address?.City ?? "Error",
            District = profile.Address?.District ?? "Error",
            Country = profile.Address?.Country ?? "Error",
            Street = profile.Address?.Street ?? "Error",
            PostalCode = profile.Address?.PostalCode ?? "Error"
        };

        return ServiceResult<UserProfileDto>.Success(result);
    }
}
