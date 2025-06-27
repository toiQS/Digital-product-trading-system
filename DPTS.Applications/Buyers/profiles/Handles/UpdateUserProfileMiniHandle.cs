using DPTS.Applications.Buyers.profiles.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyers.profiles.Handles
{
    public class UpdateUserProfileMiniHandle : IRequestHandler<UpdateUserProfileMiniCommand, ServiceResult<string>>
    {
        private readonly ILogger<UpdateUserProfileMiniHandle> _logger;
        private readonly IUserRepository _userRepository;
        private readonly ILogRepository _logRepository;

        public UpdateUserProfileMiniHandle(
            ILogger<UpdateUserProfileMiniHandle> logger,
            IUserRepository userRepository,
            ILogRepository logRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _logRepository = logRepository;
        }

        public async Task<ServiceResult<string>> Handle(UpdateUserProfileMiniCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Yêu cầu cập nhật thông tin cá nhân từ UserId: {UserId}", request.UserId);

            try
            {
                var user = await _userRepository.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    _logger.LogWarning("Không tìm thấy người dùng với ID: {UserId}", request.UserId);
                    return ServiceResult<string>.Error("Người dùng không tồn tại.");
                }

                var hasChanges = false;

                if (!string.IsNullOrWhiteSpace(request.ImageUser) && request.ImageUser != user.ImageUrl)
                {
                    user.ImageUrl = request.ImageUser;
                    hasChanges = true;
                }

                if (!string.IsNullOrWhiteSpace(request.FullName) && request.FullName != user.FullName)
                {
                    user.FullName = request.FullName;
                    hasChanges = true;
                }

                if (!string.IsNullOrWhiteSpace(request.Email) && request.Email != user.Email)
                {
                    user.Email = request.Email;
                    user.TwoFactorEnabled = false; // Nếu business rule yêu cầu như vậy
                    hasChanges = true;
                }

                if (!hasChanges)
                {
                    _logger.LogInformation("Không có thay đổi nào cần cập nhật cho UserId: {UserId}", request.UserId);
                    return ServiceResult<string>.Success("Không có thay đổi nào được thực hiện.");
                }

                user.UpdatedAt = DateTime.UtcNow;

                try
                {
                    await _userRepository.UpdateAsync(user);

                    var log = new Log()
                    {
                        LogId = Guid.NewGuid().ToString(),
                        UserId = request.UserId,
                        Action = $"Người dùng {request.UserId} đã cập nhật thông tin cá nhân.",
                        CreatedAt = DateTime.UtcNow
                    };

                    await _logRepository.AddAsync(log);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi cập nhật thông tin người dùng với ID: {UserId}", request.UserId);
                    return ServiceResult<string>.Error("Đã xảy ra lỗi khi cập nhật thông tin người dùng.");
                }

                _logger.LogInformation("Cập nhật thông tin thành công cho UserId: {UserId}", request.UserId);
                return ServiceResult<string>.Success("Thông tin cá nhân đã được cập nhật.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi hệ thống khi cập nhật thông tin cá nhân cho UserId: {UserId}", request.UserId);
                return ServiceResult<string>.Error("Đã xảy ra lỗi hệ thống.");
            }
        }
    }
}
