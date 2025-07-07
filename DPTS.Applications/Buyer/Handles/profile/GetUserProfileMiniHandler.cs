using DPTS.Applications.Buyer.Dtos.profile;
using DPTS.Applications.Buyer.Queries.profile;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles.profile;

/// <summary>
/// Handler dùng để lấy thông tin hồ sơ rút gọn (tên, email, avatar) của người dùng.
/// </summary>
public class GetUserProfileMiniHandler : IRequestHandler<GetUserProfileMiniQuery, ServiceResult<UserProfileMiniDto>>
{
    private readonly ILogger<GetUserProfileMiniHandler> _logger;
    private readonly IUserRepository _userRepository;
    private readonly IUserProfileRepository _profileRepository;

    public GetUserProfileMiniHandler(
        ILogger<GetUserProfileMiniHandler> logger,
        IUserRepository userRepository,
        IUserProfileRepository profileRepository)
    {
        _logger = logger;
        _userRepository = userRepository;
        _profileRepository = profileRepository;
    }

    public async Task<ServiceResult<UserProfileMiniDto>> Handle(GetUserProfileMiniQuery request, CancellationToken cancellationToken)
    {
        _logger.LogInformation("Handling request to get mini user profile: {UserId}", request.UserId);

        // 1. Lấy thông tin người dùng từ User table
        var user = await _userRepository.GetByIdAsync(request.UserId);
        if (user == null)
        {
            _logger.LogError("User not found: {UserId}", request.UserId);
            return ServiceResult<UserProfileMiniDto>.Error("Không tìm thấy người dùng.");
        }

        // 2. Lấy hồ sơ người dùng từ UserProfile table
        var profile = await _profileRepository.GetByUserIdAsync(user.UserId);
        if (profile == null)
        {
            _logger.LogError("User profile not found for user: {UserId}", request.UserId);
            return ServiceResult<UserProfileMiniDto>.Error("Không tìm thấy hồ sơ người dùng.");
        }

        // 3. Chuẩn bị dữ liệu trả về
        var result = new UserProfileMiniDto
        {
            FullName = profile.FullName ?? "Không có tên",
            Email = user.Email,
            ProfileImage = profile.ImageUrl ?? string.Empty
        };

        return ServiceResult<UserProfileMiniDto>.Success(result);
    }
}
