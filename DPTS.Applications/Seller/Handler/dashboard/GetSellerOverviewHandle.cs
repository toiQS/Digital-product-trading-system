﻿using DPTS.Applications.Seller.Dtos.dashboard;
using DPTS.Applications.Seller.Query.dashboard;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Bcpg;

namespace DPTS.Applications.Seller.Handler.dashboard
{
    public class GetSellerOverviewHandle : IRequestHandler<GetSellerOverviewQuery, ServiceResult<IEnumerable<SellerOverviewDto>>>
    {
        private readonly ILogger<GetSellerOverviewHandle> _logger;
        private readonly IEscrowRepository _escrowRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly IStoreRepository _storeRepository;

        public GetSellerOverviewHandle(
            ILogger<GetSellerOverviewHandle> logger,
            IEscrowRepository escrowRepository,
            IProductRepository productRepository,
            IProductReviewRepository productReviewRepository,
            IStoreRepository storeRepository)
        {
            _logger = logger;
            _escrowRepository = escrowRepository;
            _productRepository = productRepository;
            _productReviewRepository = productReviewRepository;
            _storeRepository = storeRepository;
        }

        public async Task<ServiceResult<IEnumerable<SellerOverviewDto>>> Handle(GetSellerOverviewQuery query, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Handling GetSellerOverviewQuery for SellerId: {SellerId} with CountDay: {CountDay}", query.SellerId, query.CountDay);

            if (query.CountDay != 7 && query.CountDay != 30)
            {
                _logger.LogWarning("Invalid CountDay value: {CountDay}", query.CountDay);
                return ServiceResult<IEnumerable<SellerOverviewDto>>.Error("Số ngày thống kê không hợp lệ. Chỉ hỗ trợ 7 hoặc 30 ngày.");
            }

            var store = await _storeRepository.GetByUserIdAsync(query.SellerId);
            if (store == null)
            {
                _logger.LogWarning("Store not found for SellerId: {SellerId}", query.SellerId);
                return ServiceResult<IEnumerable<SellerOverviewDto>>.Error("Không tìm thấy cửa hàng.");
            }

            var escrows = await _escrowRepository.GetAllAsync(storeId: store.StoreId);
            var products = await _productRepository.GetByStoreAsync(storeId: store.StoreId);
            var reviews = await _productReviewRepository.GetAllAsync();

            // --- RatingOveralls ---
            var joinedReviews = from p in products
                                join r in reviews on p.ProductId equals r.ProductId
                                select r.RatingOverall;

            var RatingOverallCount = joinedReviews.Count();
            var averageRatingOverall = RatingOverallCount > 0 ? Math.Round((decimal)joinedReviews.Sum() / RatingOverallCount, 1) : 0;

            var RatingOverallDto = new SellerOverviewDto
            {
                OverViewName = "Đánh giá",
                Information = $"Dựa trên {RatingOverallCount} đánh giá.",
                Value = averageRatingOverall
            };

            // --- Products ---
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

            // --- Time Range Setup ---
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

            // --- Orders & Revenue ---
            var escrowsCurrent = escrows.Where(x => x.Status == EscrowStatus.Done && x.CreatedAt >= startCurrent && x.CreatedAt <= endCurrent).ToList();
            var escrowsPrevious = escrows.Where(x => x.Status == EscrowStatus.Done && x.CreatedAt >= startPrevious && x.CreatedAt <= endPrevious).ToList();

            var orderCountCurrent = escrowsCurrent.Count;
            var orderCountPrevious = escrowsPrevious.Count;
            var revenueCurrent = escrowsCurrent.Sum(x => x.Amount);
            var revenuePrevious = escrowsPrevious.Sum(x => x.Amount);

            var orderChangePercent = orderCountPrevious > 0
                ? Math.Round((decimal)(orderCountCurrent - orderCountPrevious) / orderCountPrevious * 100, 1)
                : 100;

            var revenueChangePercent = revenuePrevious > 0
                ? Math.Round((revenueCurrent - revenuePrevious) / revenuePrevious * 100, 1)
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

            _logger.LogInformation("Successfully built overview for seller {SellerId}", query.SellerId);
            return ServiceResult<IEnumerable<SellerOverviewDto>>.Success(new[] { revenueDto, orderDto, productDto, RatingOverallDto });
        }
    }

}
