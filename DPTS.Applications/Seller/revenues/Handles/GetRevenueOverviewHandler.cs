using DPTS.Applications.Seller.revenues.Dtos;
using DPTS.Applications.Seller.revenues.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.revenues.Handles
{
    public class GetRevenueOverviewHandler : IRequestHandler<GetRevenueOverviewQuery,ServiceResult<IEnumerable<RevenueOverviewDto>>>
    {

        private readonly IEscrowRepository _ecrowRepository;
        private readonly ILogger<GetRevenueOverviewHandler> _logger;
        public GetRevenueOverviewHandler(IEscrowRepository ecrowRepository, ILogger<GetRevenueOverviewHandler> logger)
        {
            _ecrowRepository = ecrowRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<RevenueOverviewDto>>> Handle(GetRevenueOverviewQuery query, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("");
            try
            {
                // tổng doanh thu
                var escrows = await _ecrowRepository.GetsAsync(sellerId: query.SellerId, includeOrder: true);
                var totalRevenues = escrows.Sum(x => x.Amount);

                // tổng doanh thu trong tháng này
                var (thisMonthStart, thisMonthEnd) = SharedHandle.GetMonthRange(0);
                var escrowInThisMonth = await _ecrowRepository.GetsAsync(sellerId: query.SellerId, includeOrder: true, fromDate: thisMonthStart, toDate: thisMonthEnd);
                var totalRevenueInThisMonth = escrowInThisMonth.Sum(x => x.Amount);
                // tổng doanh thu trong tháng trước
                var (lastMonthStart, lastMonthEnd) = SharedHandle.GetMonthRange(1);
                var escrowInLastMonth = await _ecrowRepository.GetsAsync(sellerId: query.SellerId, includeOrder: true, fromDate: lastMonthStart, toDate: lastMonthEnd);
                var totalRevenueInLastMonth = escrowInLastMonth.Sum(x => x.Amount);
                // số lượng đơn hàng
                var countOrders = escrows.Count();
                // số lượng đơn hàng mới
                var countNewOrders = escrows.Where(x => x.CreatedAt >= DateTime.Today && x.CreatedAt < DateTime.Today.AddDays(1)).Count();
                // phần trăm so sánh doanh thu
                decimal monthlyRevenueChangeRate = ((totalRevenueInLastMonth - totalRevenueInThisMonth) / countOrders) * 100;

                return ServiceResult<IEnumerable<RevenueOverviewDto>>.Success(new List<RevenueOverviewDto>
                {
                    new RevenueOverviewDto
                    {
                        NameOverview ="Tổng doanh thu",
                        Information ="",
                        Values = totalRevenues
                    },
                    new RevenueOverviewDto
                    {
                        NameOverview = "Doanh thu tháng",
                        Information =$"{monthlyRevenueChangeRate} so với tháng trước",
                        Values = totalRevenueInThisMonth
                    },
                    new RevenueOverviewDto
                    {
                        NameOverview = "Tổng số đơn hàng",
                        Values = countOrders,
                        Information = $"{countNewOrders} đơn mới"
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("");
                return ServiceResult<IEnumerable<RevenueOverviewDto>>.Error("");
            }
        }
    }
}
