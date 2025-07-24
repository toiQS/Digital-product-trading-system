using DPTS.Applications.Admin.manage_user.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Admin.manage_user.handlers
{
    public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, ServiceResult<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogRepository _logRepository;
        private readonly ILogger<UpdateUserHandler> _logger;

        public UpdateUserHandler(IUserRepository userRepository, ILogRepository logRepository, ILogger<UpdateUserHandler> logger)
        {
            _userRepository = userRepository;
            _logRepository = logRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<string>> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling update user");
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("Not found user with Id: {id}",request.UserId);
                return ServiceResult<string>.Error("Không tìm thấy người dùng");

            }
            if (user.RoleId != "Admin")
            {
                _logger.LogError("User current don't have enough access book");
                return ServiceResult<string>.Error("Người dùng hiện tại không đủ quyền truy cập");
            }
            var log = new Log()
            {
                Action = "Cập nhật tình trạng",
                CreatedAt = DateTime.UtcNow,
                Description = $"Cập nhật tình trạng của user {request.UpdateUser.UserId}",
                LogId = Guid.NewGuid().ToString(),
                TargetId = request.UpdateUser.UserId,
                TargetType ="User",
                UserId = request.UserId,
            };
            var otherUser = await _userRepository.GetByIdAsync(request.UpdateUser.UserId);
            if (otherUser == null)
            {
                _logger.LogError("Not found user with Id: {id}", request.UpdateUser.UserId);
                return ServiceResult<string>.Error("Không tìm thấy người dùng");
            }
            try
            {
                otherUser.IsActive = request.UpdateUser.IsAvalible;
                await _userRepository.UpdateAsync(otherUser);
                await _logRepository.AddAsync(log);
                return ServiceResult<string>.Success("Cập nhật thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError("Error when update user with Id:{Id}",request.UpdateUser.UserId);
                return ServiceResult<string>.Error("Không thẻ cập nhật");
            }
        }
    }
}
