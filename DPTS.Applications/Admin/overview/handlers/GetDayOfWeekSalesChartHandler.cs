using DPTS.Applications.Admin.overview.dtos;
using DPTS.Applications.Admin.overview.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Admin.overview.handlers
{
    public class GetDayOfWeekSalesChartHandler : IRequestHandler<GetDayOfWeekSalesChartQuery, ServiceResult<ChartDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEscrowRepository _ecrowRepository;
        private readonly ILogger<GetDayOfWeekSalesChartHandler> _logger;

        public GetDayOfWeekSalesChartHandler(IUserRepository userRepository, IEscrowRepository ecrowRepository, ILogger<GetDayOfWeekSalesChartHandler> logger)
        {
            _userRepository = userRepository;
            _ecrowRepository = ecrowRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<ChartDto>> Handle(GetDayOfWeekSalesChartQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling get day of week sales chart query");
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError($"Not found user with Id:{request.UserId}");
                return ServiceResult<ChartDto>.Error("Không tìm thấy người dùng");
            }
            if (user.RoleId != "Admin")
            {
                _logger.LogError($"User current {request.UserId} don't have enough access book");
                return ServiceResult<ChartDto>.Error("Người dùng hiện tại không đủ quyền truy cập");
            }
            var result = new ChartDto();
            var (startThisWeek, endThisWeek) = SharedHandle.GetWeekRange(0);
            while (startThisWeek < endThisWeek)
            {
                var escrows = (await _ecrowRepository.GetAllAsync()).Where(x => x.CreatedAt >= startThisWeek && x.CreatedAt < startThisWeek.AddDays(1) && x.Status == Domains.EscrowStatus.Done);
                result.Nodes.Add(new NodeDto()
                {
                    Name = startThisWeek.DayOfWeek.ToString(),
                    Value = escrows.Sum(x => x.ActualAmount)
                });
            }
            return ServiceResult<ChartDto>.Success(result);
        }
    }
}
