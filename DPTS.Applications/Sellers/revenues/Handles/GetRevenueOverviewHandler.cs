using DPTS.Applications.Sellers.revenues.Dtos;
using DPTS.Applications.Sellers.revenues.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Sellers.revenues.Handles
{
    public class GetRevenueOverviewHandler : IRequestHandler<GetRevenueOverviewQuery,ServiceResult<IEnumerable<RevenueOverviewDto>>>
    {

        private readonly IEscrowRepository _ecrowRepository;
        private readonly ILogger<GetRevenueOverviewHandler> _logger;
        private readonly IStoreRepository _storeRepository;
        public GetRevenueOverviewHandler(IEscrowRepository ecrowRepository, ILogger<GetRevenueOverviewHandler> logger, IStoreRepository storeRepository)
        {
            _ecrowRepository = ecrowRepository;
            _logger = logger;
            _storeRepository = storeRepository;
        }

        public async Task<ServiceResult<IEnumerable<RevenueOverviewDto>>> Handle(GetRevenueOverviewQuery query, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Bắt đầu xử lý GetRevenueOverview cho SellerId = {SellerId}", query.SellerId);

            try
            {
                var store = await _storeRepository.GetByUserIdAsync(query.SellerId);
                if (store == null)
                {
                    _logger.LogWarning("Không tìm thấy cửa hàng cho SellerId = {SellerId}", query.SellerId);
                    return ServiceResult<IEnumerable<RevenueOverviewDto>>.Error("Không tìm thấy cửa hàng.");
                }

                var escrows = await _ecrowRepository.GetsAsync(storeId: store.StoreId, includeOrder: true);
                var totalRevenues = escrows.Sum(x => x.Amount);

                _logger.LogInformation("Tổng doanh thu: {TotalRevenue}", totalRevenues);

                var (thisMonthStart, thisMonthEnd) = SharedHandle.GetMonthRange(0);
                var escrowInThisMonth = await _ecrowRepository.GetsAsync(storeId: store.StoreId, includeOrder: true, fromDate: thisMonthStart, toDate: thisMonthEnd);
                var totalRevenueInThisMonth = escrowInThisMonth.Sum(x => x.Amount);

                _logger.LogInformation("Doanh thu tháng này ({Start} - {End}): {Revenue}", thisMonthStart, thisMonthEnd, totalRevenueInThisMonth);

                var (lastMonthStart, lastMonthEnd) = SharedHandle.GetMonthRange(1);
                var escrowInLastMonth = await _ecrowRepository.GetsAsync(storeId: store.StoreId, includeOrder: true, fromDate: lastMonthStart, toDate: lastMonthEnd);
                var totalRevenueInLastMonth = escrowInLastMonth.Sum(x => x.Amount);

                _logger.LogInformation("Doanh thu tháng trước ({Start} - {End}): {Revenue}", lastMonthStart, lastMonthEnd, totalRevenueInLastMonth);

                var countOrders = escrows.Count();
                var countNewOrders = escrows.Count(x => x.CreatedAt >= DateTime.Today && x.CreatedAt < DateTime.Today.AddDays(1));

                _logger.LogInformation("Tổng số đơn hàng: {Count}, đơn mới hôm nay: {NewOrders}", countOrders, countNewOrders);

                decimal monthlyRevenueChangeRate = totalRevenueInLastMonth == 0
                    ? 100
                    : Math.Round((totalRevenueInThisMonth - totalRevenueInLastMonth) / totalRevenueInLastMonth * 100, 1);

                _logger.LogInformation("Tỷ lệ thay đổi doanh thu tháng: {Rate}%", monthlyRevenueChangeRate);

                return ServiceResult<IEnumerable<RevenueOverviewDto>>.Success(new List<RevenueOverviewDto>
        {
            new RevenueOverviewDto
            {
                NameOverview = "Tổng doanh thu",
                Information = "",
                Values = totalRevenues
            },
            new RevenueOverviewDto
            {
                NameOverview = "Doanh thu tháng",
                Information = $"{monthlyRevenueChangeRate}% so với tháng trước",
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
                _logger.LogError(ex, "Lỗi khi xử lý GetRevenueOverview cho SellerId = {SellerId}", query.SellerId);
                return ServiceResult<IEnumerable<RevenueOverviewDto>>.Error("Không thể lấy thông tin doanh thu.");
            }
        }

    }
}
