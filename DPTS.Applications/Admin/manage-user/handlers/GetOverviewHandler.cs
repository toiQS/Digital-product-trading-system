using DPTS.Applications.Admin.manage_user.dtos;
using DPTS.Applications.Admin.manage_user.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Admin.manage_user.handlers
{
    public class GetOverviewHandler : IRequestHandler<GetOverviewQuery, ServiceResult<IEnumerable<OverviewIndexDto>>>
    {
        private readonly IUserRepository _userRepository;
        private readonly ILogger<GetOverviewHandler> _logger;

        public GetOverviewHandler(IUserRepository userRepository, ILogger<GetOverviewHandler> logger)
        {
            _userRepository = userRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<OverviewIndexDto>>> Handle(GetOverviewQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling get overview user management");
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("Not found user with Id: {id}", request.UserId);
                return ServiceResult<IEnumerable<OverviewIndexDto>>.Error("Không tìm thấy người dùng");
            }
            if (user.RoleId != "Admin")
            {
                _logger.LogInformation("User current don't have enough access books");
                return ServiceResult<IEnumerable<OverviewIndexDto>>.Error("Người dùng hiện tại không đủ quyền truy cập");
            }
            var users = await _userRepository.GetAllAsync();

            var (startThisWeek, endThisWeek) = SharedHandle.GetWeekRange(0);
            var newUser = users.Count(x => x.CreatedAt >= startThisWeek && x.CreatedAt < endThisWeek && x.RoleId != "Admin");
            var result = new List<OverviewIndexDto>()
            {
                new OverviewIndexDto()
                {
                    Name = "Tổng người dùng",
                    Value = users.Count(x => x.RoleId != "Admin")
                },
                    new()
                {
                    Name = "Người dùng hoạt động",
                    Value = users.Count(x => x.RoleId != "Admin" && x.IsActive)
                }, new OverviewIndexDto()
                {
                    Name = "Người dùng mới",
                    Value = newUser
                },
                new()
                {
                    Name = "Người dùng bị chặn",
                    Value = users.Count(x => x.RoleId != "Admin" && x.IsActive == false)
                }
            };
            return ServiceResult<IEnumerable<OverviewIndexDto>>.Success(result);
        }
    }
}
