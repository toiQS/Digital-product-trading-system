using Azure.Core;
using DPTS.Applications.Admin.overview.dtos;
using DPTS.Applications.Admin.overview.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Admin.overview.handlers
{
    public class GetUserActivityHandler : IRequestHandler<GetUserActivityQuery, ServiceResult<UserActivityDto>>
    {
        private readonly IUserRepository  _userRepository;
        private readonly IEscrowRepository _escrowRepository;
        private readonly  ILogger<GetUserActivityHandler> _logger;

        public GetUserActivityHandler(IUserRepository userRepository, IEscrowRepository escrowRepository, ILogger<GetUserActivityHandler> logger)
        {
            _userRepository = userRepository;
            _escrowRepository = escrowRepository;
            _logger = logger;
        }

        async Task<ServiceResult<UserActivityDto>> IRequestHandler<GetUserActivityQuery, ServiceResult<UserActivityDto>>.Handle(GetUserActivityQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling get user activity");
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user==null)
            {
                _logger.LogError($"Not found user with Id: {request.UserId}");
                return ServiceResult<UserActivityDto>.Error("Không tìm thấy người dùng");

            }
            if (user.RoleId != "Admin")
            {
                _logger.LogError("User current don't have enough asscess books");
                return ServiceResult<UserActivityDto>.Error("Người dùng hiện tại không đủ quyền truy cập");
            }
            var result = new UserActivityDto();
            var allUser = await _userRepository.GetAllAsync();
            var countUserActivity =allUser.Where  (x => x.RoleId != "Admin" && x.IsActive).Count();
            result.UserActivities.Add(new UserActivityIndexDto()
            {
                Name = "Người dùng đang hoạt động",
                Value = countUserActivity
            });
            var newRegister = allUser.Where(x => x.CreatedAt >= DateTime.Today && x.CreatedAt < DateTime.Today.AddDays(1)).Count();
            result.UserActivities.Add(new()
            {
                Name = "Đăng ký mới",
                Value = newRegister
            });

            var escrows = await _escrowRepository.GetAllAsync();
            var escrowsTody = escrows.Where(x => x.CreatedAt >= DateTime.Today && x.CreatedAt < DateTime.Today.AddDays(1)).Count();
            result.UserActivities.Add(new()
                {
                    Name = "Đơn hàng hôm nay",
                    Value = escrowsTody
                });

            var revenueToday = escrows.Where(x => x.CreatedAt >= DateTime.Today && x.CreatedAt < DateTime.Today.AddDays(1)).Sum(x => x.ActualAmount);
            result.UserActivities.Add(new()
            {
                Name = "Doanh thu hôm nay",
                Value = revenueToday
            });
           return ServiceResult<UserActivityDto>.Success(result);
        }
    }
}
