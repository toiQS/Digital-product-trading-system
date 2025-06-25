using DPTS.Applications.Sellers.revenues.Dtos;
using DPTS.Applications.Sellers.revenues.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Sellers.revenues.Handles
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

        public async Task<ServiceResult<IEnumerable<RevenueChartPointDto>>> Handle(GetRevenueChartQuery query, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("GeneRatingOverall revenue chart for seller: {SellerId}, last {CountDay} days", query.SellerId, query.CountDay);

            if (query.CountDay <= 0 || query.CountDay > 60)
            {
                _logger.LogWarning("Invalid CountDay: {CountDay} for seller: {SellerId}", query.CountDay, query.SellerId);
                return ServiceResult<IEnumerable<RevenueChartPointDto>>.Error("Số ngày không hợp lệ (1–60).");
            }

            try
            {
                var store = await _storeRepository.GetByUserIdAsync(query.SellerId);
                if (store == null)
                {
                    _logger.LogWarning("Store not found for seller: {SellerId}", query.SellerId);
                    return ServiceResult<IEnumerable<RevenueChartPointDto>>.Error("Không tìm thấy cửa hàng.");
                }

                var revenuePoints = new List<RevenueChartPointDto>();

                for (int i = 0; i < query.CountDay; i++)
                {
                    var date = DateTime.Today.AddDays(-i);
                    var fromDate = date;
                    var toDate = date.AddDays(1);

                    var escrows = await _escrowRepository.GetsAsync(
                        storeId: store.StoreId,
                        fromDate: fromDate,
                        toDate: toDate);

                    var revenue = escrows.Sum(x => x.Amount);

                    revenuePoints.Add(new RevenueChartPointDto
                    {
                        Date = fromDate,
                        Revenue = revenue
                    });
                }

                var sortedResult = revenuePoints.OrderBy(x => x.Date).ToList();

                _logger.LogInformation("Successfully generated revenue chart for seller: {SellerId}", query.SellerId);
                return ServiceResult<IEnumerable<RevenueChartPointDto>>.Success(sortedResult);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while geneRatingOverall revenue chart for seller: {SellerId}", query.SellerId);
                return ServiceResult<IEnumerable<RevenueChartPointDto>>.Error("Lỗi khi tạo biểu đồ doanh thu.");
            }
        }
    }
}
