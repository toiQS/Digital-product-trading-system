using DPTS.Applications.Sellers.revenues.Dtos;
using DPTS.Applications.Sellers.revenues.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Sellers.revenues.Handles
{
    public class GetRevenueOverviewHandler : IRequestHandler<GetRevenueOverviewQuery, ServiceResult<IEnumerable<RevenueOverviewDto>>>
    {
        private readonly IEscrowRepository _escrowRepository;
        private readonly ILogger<GetRevenueOverviewHandler> _logger;
        private readonly IStoreRepository _storeRepository;

        public GetRevenueOverviewHandler(
            IEscrowRepository escrowRepository,
            ILogger<GetRevenueOverviewHandler> logger,
            IStoreRepository storeRepository)
        {
            _escrowRepository = escrowRepository;
            _logger = logger;
            _storeRepository = storeRepository;
        }

        public async Task<ServiceResult<IEnumerable<RevenueOverviewDto>>> Handle(GetRevenueOverviewQuery query, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Processing GetRevenueOverview for seller: {SellerId}", query.SellerId);

            // 1. Tìm cửa hàng theo sellerId
            var store = await _storeRepository.GetByUserIdAsync(query.SellerId);
            if (store == null)
            {
                _logger.LogWarning("Store not found for seller: {SellerId}", query.SellerId);
                return ServiceResult<IEnumerable<RevenueOverviewDto>>.Error("Không tìm thấy cửa hàng.");
            }

            // 2. Lấy toàn bộ escrow liên quan đến store
            var escrows = await _escrowRepository.GetAllAsync(storeId: store.StoreId, includeOrder: true);

            // 3. Tính tổng doanh thu
            var totalRevenue = escrows.Sum(x => x.Amount);
            _logger.LogInformation("Total revenue: {Revenue}", totalRevenue);

            // 4. Doanh thu trong tháng hiện tại
            var (thisMonthStart, thisMonthEnd) = SharedHandle.GetMonthRange(0);
            var thisMonthRevenue = escrows
                .Where(x => x.CreatedAt >= thisMonthStart && x.CreatedAt < thisMonthEnd)
                .Sum(x => x.Amount);
            _logger.LogInformation("This month's revenue ({Start} - {End}): {Revenue}", thisMonthStart, thisMonthEnd, thisMonthRevenue);

            // 5. Doanh thu tháng trước
            var (lastMonthStart, lastMonthEnd) = SharedHandle.GetMonthRange(1);
            var lastMonthRevenue = escrows
                .Where(x => x.CreatedAt >= lastMonthStart && x.CreatedAt < lastMonthEnd)
                .Sum(x => x.Amount);
            _logger.LogInformation("Last month's revenue ({Start} - {End}): {Revenue}", lastMonthStart, lastMonthEnd, lastMonthRevenue);

            // 6. Tính tỉ lệ thay đổi doanh thu
            decimal revenueChangeRate = lastMonthRevenue == 0
                ? 100
                : Math.Round((thisMonthRevenue - lastMonthRevenue) / lastMonthRevenue * 100, 1);
            _logger.LogInformation("Monthly revenue change rate: {Rate}%", revenueChangeRate);

            // 7. Đếm đơn hàng và đơn mới hôm nay
            var totalOrders = escrows.Count();
            var newOrdersToday = escrows.Count(x => x.CreatedAt.Date == DateTime.Today);
            _logger.LogInformation("Total orders: {Count}, New today: {New}", totalOrders, newOrdersToday);

            // 8. Trả kết quả
            var overview = new List<RevenueOverviewDto>
            {
                new RevenueOverviewDto
                {
                    NameOverview = "Tổng doanh thu",
                    Values = totalRevenue,
                    Information = ""
                },
                new RevenueOverviewDto
                {
                    NameOverview = "Doanh thu tháng",
                    Values = thisMonthRevenue,
                    Information = $"{revenueChangeRate}% so với tháng trước"
                },
                new RevenueOverviewDto
                {
                    NameOverview = "Tổng số đơn hàng",
                    Values = totalOrders,
                    Information = $"{newOrdersToday} đơn mới hôm nay"
                }
            };

            return ServiceResult<IEnumerable<RevenueOverviewDto>>.Success(overview);
        }
    }
}
