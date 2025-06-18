using DPTS.Applications.Seller.revenues.Dtos;
using DPTS.Applications.Seller.revenues.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.revenues.Handles
{
    public class GetRevenueChartHandler : IRequestHandler<GetRevenueChartQuery, ServiceResult<IEnumerable<RevenueChartPointDto>>>
    {
        private readonly IEscrowRepository _escrowRepository;
        private readonly ILogger<GetRevenueChartHandler> _logger;
        public GetRevenueChartHandler(IEscrowRepository escrowRepository, ILogger<GetRevenueChartHandler> logger)
        {
            _escrowRepository = escrowRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<RevenueChartPointDto>>> Handle(GetRevenueChartQuery query, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("");
            try
            {
                var start = DateTime.Today;
                var end = DateTime.Today.AddDays(1);
                var result = new List<RevenueChartPointDto>();
                for (var i = 0; i <= query.CountDay; i++)
                {
                    var revenue = (await _escrowRepository.GetsAsync(sellerId: query.SellerId, fromDate: start, toDate: end)).Sum(x => x.Amount);
                    var point = new RevenueChartPointDto
                    {
                        Date = start,
                        Revenue = revenue,
                    };
                    result.Add(point);
                    start = start.AddDays(-1);
                    end = end.AddDays(-1);
                }
                return ServiceResult<IEnumerable<RevenueChartPointDto>>.Success(result.OrderBy(x => x.Date));
            }
            catch (Exception ex)
            {
                _logger.LogError("");
                return ServiceResult<IEnumerable<RevenueChartPointDto>>.Error("");
            }
        }

    }
}
