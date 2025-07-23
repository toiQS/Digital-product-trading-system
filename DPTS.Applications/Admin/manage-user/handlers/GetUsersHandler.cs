using DPTS.Applications.Admin.manage_user.dtos;
using DPTS.Applications.Admin.manage_user.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Admin.manage_user.handlers
{
    public class GetUsersHandler : IRequestHandler<GetUsersQuery, ServiceResult<UserDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IUserProfileRepository _profileRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly ILogger<GetUsersHandler> _logger;
        public async Task<ServiceResult<UserDto>> Handle(GetUsersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling get users with condition");
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("Not found user with Id: {id}", request.UserId);
                return ServiceResult<UserDto>.Error("Không tìm thấy người dùng");
            }
            if (user.RoleId != "Admin")
            {
                _logger.LogInformation("User current don't have enough access books");
                return ServiceResult<UserDto>.Error("Người dùng hiện tại không đủ quyền truy cập");
            }
            var users = await _userRepository.GetAllAsync();
            if (request.Condition.Text != null)
            {
                //users = users.Where(x => )
            }
            throw new NotImplementedException();
        }
    }
}
