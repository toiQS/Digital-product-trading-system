using DPTS.Applications.Seller.Dtos.revenue;
using DPTS.Applications.Seller.Query.revenue;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.Handler.revenue
{
    public class GetRevenueChartHandler : IRequestHandler<GetRevenueChartQuery, ServiceResult<IEnumerable<RevenueChartPointDto>>>
    {
        private readonly IEscrowRepository _escrowRepository;
        private readonly ILogger<GetRevenueChartHandler> _logger;
        private readonly IStoreRepository _storeRepository;

        public GetRevenueChartHandler(
            IEscrowRepository escrowRepository,
            ILogger<GetRevenueChartHandler> logger,
            IStoreRepository storeRepository)
        {
            _escrowRepository = escrowRepository;
            _logger = logger;
            _storeRepository = storeRepository;
        }

        public async Task<ServiceResult<IEnumerable<RevenueChartPointDto>>> Handle(GetRevenueChartQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Generating revenue chart for seller: {SellerId}, last {CountDay} days", query.SellerId, query.CountDay);

            // Validate số ngày hợp lệ (1–60)
            if (query.CountDay <= 0 || query.CountDay > 60)
            {
                _logger.LogWarning("Invalid CountDay: {CountDay} for seller: {SellerId}", query.CountDay, query.SellerId);
                return ServiceResult<IEnumerable<RevenueChartPointDto>>.Error("Số ngày không hợp lệ (chỉ chấp nhận trong khoảng 1–60 ngày).");
            }

            try
            {
                // Lấy cửa hàng tương ứng với người bán
                var store = await _storeRepository.GetByUserIdAsync(query.SellerId);
                if (store == null)
                {
                    _logger.LogWarning("Store not found for seller: {SellerId}", query.SellerId);
                    return ServiceResult<IEnumerable<RevenueChartPointDto>>.Error("Không tìm thấy cửa hàng của người bán.");
                }

                var revenuePoints = new List<RevenueChartPointDto>();

                // Duyệt từng ngày để tính tổng doanh thu
                for (int i = 0; i < query.CountDay; i++)
                {
                    var fromDate = DateTime.Today.AddDays(-i);
                    var toDate = fromDate.AddDays(1);

                    // Lấy các giao dịch escrow trong ngày đó
                    var escrowsInDay = (await _escrowRepository.GetAllAsync(storeId:store.StoreId))
                        .Where(x => x.CreatedAt >= fromDate && x.CreatedAt < toDate);

                    // Tổng doanh thu trong ngày
                    var totalRevenue = escrowsInDay.Sum(x => x.Amount);

                    // Ghi nhận điểm dữ liệu
                    revenuePoints.Add(new RevenueChartPointDto
                    {
                        Date = fromDate,
                        Revenue = totalRevenue
                    });
                }

                // Sắp xếp theo ngày tăng dần
                var sortedResult = revenuePoints.OrderBy(x => x.Date).ToList();

                _logger.LogInformation("Successfully generated revenue chart for seller: {SellerId}", query.SellerId);
                return ServiceResult<IEnumerable<RevenueChartPointDto>>.Success(sortedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while generating revenue chart for seller: {SellerId}", query.SellerId);
                return ServiceResult<IEnumerable<RevenueChartPointDto>>.Error("Đã xảy ra lỗi khi tạo biểu đồ doanh thu.");
            }
        }
    }
}
