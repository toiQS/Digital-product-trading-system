using DPTS.Applications.Admin.overview.dtos;
using DPTS.Applications.Admin.overview.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Admin.overview.handlers
{
    public class GetRecentActivityHandler : IRequestHandler<GetRecentActivityQuery, ServiceResult<IEnumerable<RecentlyActivityDto>>>
    {
        private readonly ILogger<GetRecentActivityHandler> _logger;
        private readonly IUserRepository  _userRepository;
        private readonly ILogRepository _logRepository;

        public GetRecentActivityHandler(ILogger<GetRecentActivityHandler> logger, IUserRepository userRepository, ILogRepository logRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _logRepository = logRepository;
        }

        public async Task<ServiceResult<IEnumerable<RecentlyActivityDto>>> Handle(GetRecentActivityQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling get recently log");
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError($"Not found user with Id:{request.UserId}");
                return ServiceResult<IEnumerable<RecentlyActivityDto>>.Error("Không tìm thấy người dùng");
            }
            if (user.RoleId != "Admin")
            {
                _logger.LogError($"User current don't have enough access books");
                return ServiceResult<IEnumerable<RecentlyActivityDto>>.Error("Người dùng hiện tại không đủ quyền truy cập");
            }
            var logs = (await _logRepository.GetAllAsync()).OrderByDescending(x => x.CreatedAt).Take(10);

            var result = logs.Select(x => new RecentlyActivityDto()
            {
                Name = x.Action,
                CreateAt = x.CreatedAt,
                Description = x.Description,
            }).ToList();
            return ServiceResult<IEnumerable<RecentlyActivityDto>>.Success(result);
        }
    }
}
