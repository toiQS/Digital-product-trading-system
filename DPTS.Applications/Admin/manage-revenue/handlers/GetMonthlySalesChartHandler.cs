using DPTS.Applications.Admin.manage_revenue.dtos;
using DPTS.Applications.Admin.manage_revenue.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Admin.manage_revenue.handlers
{
    public class GetMonthlySalesChartHandler : IRequestHandler<GetMonthlySalesChartQuery, ServiceResult<ChartDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEscrowRepository _ecrowRepository;
        private readonly ILogger<GetMonthlySalesChartHandler> _logger;

        public GetMonthlySalesChartHandler(IUserRepository userRepository, IEscrowRepository ecrowRepository, ILogger<GetMonthlySalesChartHandler> logger)
        {
            _userRepository = userRepository;
            _ecrowRepository = ecrowRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<ChartDto>> Handle(GetMonthlySalesChartQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling get monthly sales chart");
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
            var (startThisYear, endThisYear) = SharedHandle.GetYearRange(0);
            while (startThisYear < endThisYear)
            {
                var escrows = (await _ecrowRepository.GetAllAsync()).Where(x => x.CreatedAt >= startThisYear && x.CreatedAt < startThisYear.AddDays(1) && x.Status == Domains.EscrowStatus.Done);
                result.Nodes.Add(new NodeDto()
                {
                    Name = startThisYear.Month.ToString(),
                    Value = escrows.Sum(x => x.ActualAmount)
                });
            }
            return ServiceResult<ChartDto>.Success(result);
        }
    }
}
