using DPTS.Applications.Admin.manage_revenue.dtos;
using DPTS.Applications.Admin.manage_revenue.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Admin.manage_revenue.handlers
{
    public class GetOverviewRevenueHandler : IRequestHandler<GetOverviewRevenueQuery, ServiceResult<OverViewRevenueDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IEscrowRepository _escrowRepository;
        private readonly ILogger<GetOverviewRevenueHandler> _logger;

        public GetOverviewRevenueHandler(IUserRepository userRepository,
                                         IEscrowRepository escrowRepository,
                                         ILogger<GetOverviewRevenueHandler> logger)
        {
            _userRepository = userRepository;
            _escrowRepository = escrowRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<OverViewRevenueDto>> Handle(GetOverviewRevenueQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling get overview revenue");

            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("Not found user with Id: {id}", request.UserId);
                return ServiceResult<OverViewRevenueDto>.Error("Không tìm thấy người dùng");
            }

            if (user.RoleId != "Admin")
            {
                _logger.LogInformation("User current don't have enough access books");
                return ServiceResult<OverViewRevenueDto>.Error("Người dùng hiện tại không đủ quyền truy cập");
            }

            var escrows = (await _escrowRepository.GetAllAsync())
                .Where(x => x.Status == Domains.EscrowStatus.Done)
                .ToList();

            var sumAmount = escrows.Sum(x => x.Amount);
            var sumPlatformFee = escrows.Sum(x => x.PlatformFeeAmount);

            var (startThisMonth, endThisMonth) = SharedHandle.GetMonthRange(0);
            var (startLastMonth, endLastMonth) = SharedHandle.GetMonthRange(-1);

            var sumAmountThisMonth = escrows
                .Where(x => x.CreatedAt >= startThisMonth && x.CreatedAt < endThisMonth)
                .Sum(x => x.Amount);

            var sumAmountLastMonth = escrows
                .Where(x => x.CreatedAt >= startLastMonth && x.CreatedAt < endLastMonth)
                .Sum(x => x.Amount);

            string growthRate;
            string growthInfo;

            if (sumAmountLastMonth != 0)
            {
                var growthPercentage = (sumAmountThisMonth - sumAmountLastMonth) / sumAmountLastMonth * 100;
                growthRate = growthPercentage.ToString("0.##") + " %";
                growthInfo = ConvertUnit(sumAmountThisMonth - sumAmountLastMonth) + " so với tháng trước";
            }
            else
            {
                growthRate = "Không có dữ liệu tháng trước";
                growthInfo = "Không thể so sánh";
            }

            var averageValue = escrows.Any()
                ? ConvertUnit(escrows.Average(x => x.Amount))
                : "Không có dữ liệu";

            var result = new OverViewRevenueDto
            {
                OverviewRevenueIndexDtos = new List<OverviewRevenueIndexDto>
                {
                    new OverviewRevenueIndexDto
                    {
                        Name = "Tổng doanh thu",
                        Value = ConvertUnit(sumAmount)
                    },
                    new OverviewRevenueIndexDto
                    {
                        Name = "Phí nền tảng",
                        Value = ConvertUnit(sumPlatformFee)
                    },
                    new OverviewRevenueIndexDto
                    {
                        Name = "Tăng trưởng tháng",
                        Value = growthRate,
                        Information = growthInfo
                    },
                    new OverviewRevenueIndexDto
                    {
                        Name = "Giá trị trung bình",
                        Value = averageValue
                    }
                }
            };

            return ServiceResult<OverViewRevenueDto>.Success(result);
        }

        private string ConvertUnit(decimal value)
        {
            if (value < 1_000_000)
                return $"{value:N0} vnđ";

            if (value >= 1_000_000 && value < 1_000_000_000)
                return $"{value / 1_000_000:N2} triệu vnđ";

            if (value >= 1_000_000_000 && value < 10_000_000_000)
                return $"{value / 1_000_000_000:N2} tỉ vnđ";

            return "Vượt hạn mức";
        }
    }
}
