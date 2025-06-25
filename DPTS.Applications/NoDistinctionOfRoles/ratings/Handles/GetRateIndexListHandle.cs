using DPTS.Applications.NoDistinctionOfRoles.ratings.Dtos;
using DPTS.Applications.NoDistinctionOfRoles.ratings.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.NoDistinctionOfRoles.ratings.Handles
{
    public class GetRateIndexListHandle : IRequestHandler<GetRateIndexListQuery, ServiceResult<IEnumerable<RateIndexListDto>>>
    {
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly ILogger<GetRateIndexListHandle> _logger;

        public GetRateIndexListHandle(IProductReviewRepository productReviewRepository, ILogger<GetRateIndexListHandle> logger)
        {
            _logger = logger;
            _productReviewRepository = productReviewRepository;
        }
        public async Task<ServiceResult<IEnumerable<RateIndexListDto>>> Handle(GetRateIndexListQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Bắt đầu xử lý GetRateIndexList cho query: {Query}", request);

            try
            {
                var reviews = await _productReviewRepository.GetsAsync();
                _logger.LogInformation("Đã lấy được {Count} đánh giá từ repository.", reviews.Count());

                List<QueryResult> queryResults;
                try
                {
                    // Tính toán điểm đánh giá trung bình theo từng sản phẩm
                    var query = from review in reviews
                                group review by review.ProductId into grouped
                                select new QueryResult
                                {
                                    ProductId = grouped.Key,
                                    RatingOverallAverate = grouped.Average(a => a.RatingOverall),
                                };
                    queryResults = query.ToList();
                    _logger.LogInformation("Đã tính toán xong trung bình đánh giá cho {Count} sản phẩm.", queryResults.Count);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi tính toán điểm trung bình đánh giá cho các sản phẩm.");
                    return ServiceResult<IEnumerable<RateIndexListDto>>.Error("Không thể tính toán trung bình đánh giá.");
                }

                // Lọc và phân loại các đánh giá
                var vote1 = queryResults.Where(x => x.RatingOverallAverate >= 1 && x.RatingOverallAverate <= 2).ToList();
                var vote2 = queryResults.Where(x => x.RatingOverallAverate >= 2 && x.RatingOverallAverate <= 3).ToList();
                var vote3 = queryResults.Where(x => x.RatingOverallAverate >= 3 && x.RatingOverallAverate < 4).ToList();
                var vote4 = queryResults.Where(x => x.RatingOverallAverate >= 4 && x.RatingOverallAverate < 5).ToList();
                var vote5 = queryResults.Where(x => x.RatingOverallAverate == 5).ToList();

                _logger.LogInformation("Phân loại thành công: {Vote1Count} sản phẩm có điểm từ 1-2, {Vote2Count} sản phẩm có điểm từ 2-3, {Vote3Count} sản phẩm có điểm từ 3-4, {Vote4Count} sản phẩm có điểm từ 4-5, {Vote5Count} sản phẩm có điểm 5.",
                    vote1.Count, vote2.Count, vote3.Count, vote4.Count, vote5.Count);

                var result = new List<RateIndexListDto>
                {
                    new RateIndexListDto { RatingOverall = 1, Count = vote1.Count },
                    new RateIndexListDto { RatingOverall = 2, Count = vote2.Count },
                    new RateIndexListDto { RatingOverall = 3, Count = vote3.Count },
                    new RateIndexListDto { RatingOverall = 4, Count = vote4.Count },
                    new RateIndexListDto { RatingOverall = 5, Count = vote5.Count }
                };

                return ServiceResult<IEnumerable<RateIndexListDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi không xác định trong GetRateIndexList.");
                return ServiceResult<IEnumerable<RateIndexListDto>>.Error("Không thể lấy thông tin đánh giá.");
            }
        }

        private class QueryResult
        {
            public string ProductId { get; set; } = string.Empty;
            public double RatingOverallAverate { get; set; }
        }

    }
}
