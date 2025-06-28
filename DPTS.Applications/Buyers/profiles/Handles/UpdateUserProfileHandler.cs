using DPTS.Applications.Buyers.profiles.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyers.profiles.Handles
{
    public class UpdateUserProfileHandler : IRequestHandler<UpdateUserProfileCommand, ServiceResult<string>>
    {
        private readonly ILogger<UpdateUserProfileHandler> _logger;
        private readonly IUserRepository _userRepository;
        private readonly ILogRepository _logRepository;

        public UpdateUserProfileHandler(
            ILogger<UpdateUserProfileHandler> logger,
            IUserRepository userRepository,
            ILogRepository logRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _logRepository = logRepository;
        }

        public async Task<ServiceResult<string>> Handle(UpdateUserProfileCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Bắt đầu cập nhật hồ sơ người dùng cho UserId: {UserId}", request.UserId);

            try
            {
                var user = await _userRepository.GetByIdAsync(request.UserId);
                if (user == null)
                {
                    _logger.LogWarning("Không tìm thấy người dùng với ID: {UserId}", request.UserId);
                    return ServiceResult<string>.Error("Người dùng không tồn tại.");
                }

                //// Đảm bảo Address không null
                //if (user.Address == null)
                //    user.Address = new Address();

                //user.FullName = request.FullName ?? user.FullName;
                //user.Phone = request.PhoneNumber ?? user.Phone;
                //user.Address.Street = request.Street ?? user.Address.Street;
                //user.Address.City = request.City ?? user.Address.City;
                //user.Address.District = request.District ?? user.Address.District;
                //user.Address.Country = request.Country ?? user.Address.Country;
                //user.Address.PostalCode = request.PostalCode ?? user.Address.PostalCode;
                //user.UpdatedAt = DateTime.UtcNow;

                try
                {
                    await _userRepository.UpdateAsync(user);

                    var log = new Log()
                    {
                        LogId = Guid.NewGuid().ToString(),
                        UserId = request.UserId,
                        CreatedAt = DateTime.UtcNow,
                        Action = $"Người dùng {request.UserId} đã cập nhật hồ sơ cá nhân."
                    };
                    await _logRepository.AddAsync(log);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi cập nhật thông tin người dùng với ID: {UserId}", request.UserId);
                    return ServiceResult<string>.Error("Đã xảy ra lỗi khi cập nhật thông tin.");
                }

                _logger.LogInformation("Cập nhật hồ sơ thành công cho UserId: {UserId}", request.UserId);
                return ServiceResult<string>.Success("Cập nhật thông tin thành công.");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi hệ thống khi cập nhật hồ sơ người dùng.");
                return ServiceResult<string>.Error("Đã xảy ra lỗi hệ thống.");
            }
        }
    }
}
