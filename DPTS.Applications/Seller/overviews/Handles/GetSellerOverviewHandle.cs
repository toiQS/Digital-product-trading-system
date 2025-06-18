using DPTS.Applications.Seller.overviews.Dtos;
using DPTS.Applications.Seller.overviews.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.overviews.Handles
{
    public class GetSellerOverviewHandle : IRequestHandler<GetSellerOverviewQuery, ServiceResult<IEnumerable<SellerOverviewDto>>>
    {
        private readonly ILogger<GetSellerOverviewHandle> _logger;
        private readonly IEscrowRepository _escrowRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductReviewRepository _productReviewRepository;

        public GetSellerOverviewHandle(
            ILogger<GetSellerOverviewHandle> logger,
            IEscrowRepository escrowRepository,
            IProductRepository productRepository,
            IProductReviewRepository productReviewRepository)
        {
            _logger = logger;
            _escrowRepository = escrowRepository;
            _productRepository = productRepository;
            _productReviewRepository = productReviewRepository;
        }

        public async Task<ServiceResult<IEnumerable<SellerOverviewDto>>> Handle(GetSellerOverviewQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Tổng hợp thống kê tổng quan cho seller {SellerId}", query.SellerId);

                var escrows = await _escrowRepository.GetsAsync(sellerId: query.SellerId);
                var products = await _productRepository.GetsAsync(sellerId: query.SellerId);
                var reviews = await _productReviewRepository.GetsAsync();

                // Đánh giá trung bình
                var joinedReviews = from p in products
                                    join r in reviews on p.ProductId equals r.ProductId
                                    select new { r.Rating };

                var ratingCount = joinedReviews.Count();
                var ratingSum = joinedReviews.Sum(x => x.Rating);
                var averageRating = ratingCount > 0 ? Math.Round((decimal)ratingSum / ratingCount, 1) : 0;

                var ratingDto = new SellerOverviewDto
                {
                    OverViewName = "Đánh giá",
                    Information = $"Dựa trên {ratingCount} đánh giá.",
                    Value = averageRating
                };

                // Sản phẩm
                var productCount = products.Count();
                var productActive = products.Count(x => x.Status == ProductStatus.Available);
                var productPending = products.Count(x => x.Status == ProductStatus.Pending);
                var productBlocked = products.Count(x => x.Status == ProductStatus.Blocked);

                var productDto = new SellerOverviewDto
                {
                    OverViewName = "Sản phẩm",
                    Value = productCount,
                    Information = $"{productActive} đang hoạt động, {productPending} chờ duyệt, {productBlocked} bị chặn"
                };

                // Doanh thu & Đơn hàng
                DateTime startCurrent, endCurrent, startPrevious, endPrevious;
                string periodText;

                if (query.CountDay == 7)
                {
                    (startCurrent, endCurrent) = SharedHandle.GetWeekRange(0);
                    (startPrevious, endPrevious) = SharedHandle.GetWeekRange(1);
                    periodText = "tuần";
                }
                else
                {
                    (startCurrent, endCurrent) = SharedHandle.GetMonthRange(0);
                    (startPrevious, endPrevious) = SharedHandle.GetMonthRange(1);
                    periodText = "tháng";
                }

                var escrowsCurrent = escrows.Where(x => x.Status == EscrowStatus.Done && x.CreatedAt >= startCurrent && x.CreatedAt <= endCurrent).ToList();
                var escrowsPrevious = escrows.Where(x => x.Status == EscrowStatus.Done && x.CreatedAt >= startPrevious && x.CreatedAt <= endPrevious).ToList();

                var orderCountCurrent = escrowsCurrent.Count;
                var orderCountPrevious = escrowsPrevious.Count;

                var revenueCurrent = escrowsCurrent.Sum(x => x.Amount);
                var revenuePrevious = escrowsPrevious.Sum(x => x.Amount);

                var orderChangePercent = orderCountPrevious != 0
                    ? Math.Round((decimal)(orderCountCurrent - orderCountPrevious) / orderCountPrevious * 100, 1)
                    : 100;

                var revenueChangePercent = revenuePrevious != 0
                    ? Math.Round((decimal)(revenueCurrent - revenuePrevious) / revenuePrevious * 100, 1)
                    : 100;

                var orderDto = new SellerOverviewDto
                {
                    OverViewName = "Đơn hàng",
                    Value = orderCountCurrent,
                    Information = $"{orderChangePercent}% so với {periodText} trước"
                };

                var revenueDto = new SellerOverviewDto
                {
                    OverViewName = "Doanh thu",
                    Value = revenueCurrent,
                    Information = $"{revenueChangePercent}% so với {periodText} trước"
                };

                return ServiceResult<IEnumerable<SellerOverviewDto>>.Success(new[] { revenueDto, orderDto, productDto, ratingDto });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi thống kê tổng quan seller {SellerId}", query.SellerId);
                return ServiceResult<IEnumerable<SellerOverviewDto>>.Error("Không thể lấy thống kê tổng quan.");
            }
        }
    }
}
