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

        public GetOverviewRevenueHandler(IUserRepository userRepository, IEscrowRepository escrowRepository, ILogger<GetOverviewRevenueHandler> logger)
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
            
            var escrows = (await _escrowRepository.GetAllAsync()).Where(x => x.Status == Domains.EscrowStatus.Done);
            var sumAmount = escrows.Sum(x => x.Amount);
            var sumPlatfromFee = escrows.Sum(x => x.PlatformFeeAmount);
            var (startThisMonth, endThisMonth) = SharedHandle.GetMonthRange(0);
            var (startLastMonth, endLastMonth) = SharedHandle.GetMonthRange(-1);
            var sumAmountThisMonth = escrows.Where(x => x.CreatedAt >= startThisMonth && x.CreatedAt < endThisMonth).Sum(x => x.Amount);
            var sumAmountLastMonth = escrows.Where(x => x.CreatedAt >= startLastMonth && x.CreatedAt < endLastMonth).Sum(x => x.Amount);
            var result = new OverViewRevenueDto()
            {
                OverviewRevenueIndexDtos = new List<OverviewRevenueIndexDto>()
                {
                    new OverviewRevenueIndexDto()
                    {
                        Name = "Tổng doanh thu",
                        Value = ConvertUnit(sumAmount)
                    },
                    new OverviewRevenueIndexDto()
                    {
                        Name = "Phí nền tảng",
                        Value = ConvertUnit(sumPlatfromFee)
                    },
                    new OverviewRevenueIndexDto()
                    {
                        Name = "Tăng trường tháng",
                        Value = (sumAmountThisMonth - sumAmountLastMonth)/sumAmountLastMonth*100 + " %",
                        Information = ConvertUnit(sumAmountThisMonth - sumAmountLastMonth) + " so với tháng trước"
                    },
                    new OverviewRevenueIndexDto()
                    {
                        Name = "Giá trị trung bình",
                        Value = ConvertUnit(escrows.Average(x => x.Amount))
                    }
                }
            };
            throw new NotImplementedException();
        }
        private string ConvertUnit(decimal value)
        {
            if (value < 1000000)
                return value + " vnđ";
            if (value >= 1000000 && value< 9999999)
                return value / 1000000 + " triệu vnđ";

            if (value >= 1000000000 && value < 9999999999)
                return value / 1000000 + " tỉ vnđ";
            else return "Vượt hạn mức";
        }
    }
}
