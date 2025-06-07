using DPTS.Applications.Dtos;
using DPTS.Applications.Interfaces;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DPTS.Applications.Implements
{
    public class StatictisService : IStatisticService
    {
        private readonly ApplicationDbContext _context;

        public StatictisService(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Seller

        #region Doanh thu

        // thống kê doanh thu so với ngày trước
        public async Task<ServiceResult<StatisticSellerIndexDto>> SalesRevenueInDayAsync(string sellerId)
        {
            var escrows = await _context.Escrows
                .Where(x => x.SellerId == sellerId && x.Status == EscrowStatus.Done)
                .ToListAsync();

            var salesRevenueToday = CalculateTotalAmount(escrows, DateTime.Today, DateTime.Today);
            var salesRevenueYesterday = CalculateTotalAmount(escrows, DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-1));

            var percentage = salesRevenueYesterday == 0
                ? (salesRevenueToday > 0 ? 100 : 0)
                : (double)((salesRevenueToday - salesRevenueYesterday) * 100 / salesRevenueYesterday);
            return ServiceResult<StatisticSellerIndexDto>.Success(new StatisticSellerIndexDto
            {
                StatisticName = "Doanh thu ngày",
                Value = salesRevenueToday,
                Infomation = $"{percentage} so với ngày trước."
            });
        }
        // Thống kê doanh thu trong tuần so với tuần trước
        public async Task<ServiceResult<StatisticSellerIndexDto>> SalesRevenueInWeekAsync(string sellerId)
        {

            var (startThisWeek, endThisWeek) = GetWeekRange(0);
            var (startLastWeek, endLastWeek) = GetWeekRange(-1);

            var escrows = await _context.Escrows
                .Where(x => x.SellerId == sellerId && x.Status == EscrowStatus.Done)
                .ToListAsync();

            var salesRevenueThisWeek = CalculateTotalAmount(escrows, startThisWeek, endThisWeek);
            var salesRevenueLastWeek = CalculateTotalAmount(escrows, startLastWeek, endLastWeek);

            double percentage = salesRevenueLastWeek == 0
                ? (salesRevenueThisWeek > 0 ? 100 : 0)
                : (double)((salesRevenueThisWeek - salesRevenueLastWeek) * 100 / salesRevenueLastWeek);

            return ServiceResult<StatisticSellerIndexDto>.Success(new StatisticSellerIndexDto
            {
                StatisticName = "Doanh thu tuần",
                Value = salesRevenueThisWeek,
                Infomation = $"{percentage} so với tuần trước."
            });
        }
        // Thống kê doanh thu trong tháng so với tháng trước
        public async Task<ServiceResult<StatisticSellerIndexDto>> SalesRevenueInMonthAsync(string sellerId)
        {
            var escrows = await _context.Escrows
                .Where(x => x.SellerId == sellerId && x.Status == EscrowStatus.Done)
                .ToListAsync();

            var (startThisMonth, endThisMonth) = GetMonthRange(0);
            var (startLastMonth, endLastMonth) = GetMonthRange(-1);

            var salesRevenueThisMonth = CalculateTotalAmount(escrows, startThisMonth, endThisMonth);
            var salesRevenueLastMonth = CalculateTotalAmount(escrows, startLastMonth, endLastMonth);
            return ServiceResult<StatisticSellerIndexDto>.Success(new StatisticSellerIndexDto
            {
                StatisticName = "Doanh thu tháng",
                Value = salesRevenueThisMonth,
                Infomation = $"{(salesRevenueLastMonth == 0 ? (salesRevenueThisMonth > 0 ? 100 : 0) : (double)((salesRevenueThisMonth - salesRevenueLastMonth) * 100 / salesRevenueLastMonth))} so với tháng trước."
            });
        }

        // Thống kê số đơn hàng đã bán trong năm so với năm trước
        public async Task<ServiceResult<StatisticSellerIndexDto>> SaleRevenueInYearAsync(string sellerId)
        {
            var escrows = await _context.Escrows
                .Where(x => x.SellerId == sellerId && x.Status == EscrowStatus.Done)
                .ToListAsync();
            var (startThisYear, endThisYear) = GetYearRange(0);
            var (startLastYear, endLastYear) = GetYearRange(-1);
            var salesRevenueThisYear = CalculateTotalAmount(escrows, startThisYear, endThisYear);
            var salesRevenueLastYear = CalculateTotalAmount(escrows, startLastYear, endLastYear);
            return ServiceResult<StatisticSellerIndexDto>.Success(new StatisticSellerIndexDto
            {
                StatisticName = "Doanh thu năm",
                Value = salesRevenueThisYear,
                Infomation = $"{(salesRevenueLastYear == 0 ? (salesRevenueThisYear > 0 ? 100 : 0) : (double)((salesRevenueThisYear - salesRevenueLastYear) * 100 / salesRevenueLastYear))} so với năm trước."
            });
        }
        #endregion

        

        // Tính điểm đánh giá trung bình dựa trên toàn bộ sản phẩm của người bán
        public async Task<ServiceResult<StatisticSellerIndexDto>> RatingAsync(string sellerId)
        {
            var products = await _context.Products
                .Where(x => x.SellerId == sellerId)
                .Include(x => x.Reviews)
                .ToListAsync();

            var ratingSum = products.Sum(p => p.Reviews.Sum(r => r.Rating));
            var ratingCount = products.Sum(p => p.Reviews.Count);
            var averageRating = (ratingCount > 0 ? (decimal) ratingSum / ratingCount : 0);

            return ServiceResult<StatisticSellerIndexDto>.Success(new StatisticSellerIndexDto
            {
                StatisticName = "Đánh giá",
                Infomation = $"Dựa trên {ratingCount} đánh giá.",
                Value = averageRating
            });
        }

        // Thống kê số lượng sản phẩm và trạng thái của người bán trong tuần
        public async Task<ServiceResult<StatisticSellerIndexDto>> ProductOfSellerInWeekAsync(string sellerId)
        {
            var products = await _context.Products
                .Where(x => x.SellerId == sellerId)
                .ToListAsync();

            var info = $"{products.Count(x => x.Status == ProductStatus.Available)} đang kinh doanh, " +
                       $"{products.Count(x => x.Status == ProductStatus.Pending)} đang chờ duyệt, " +
                       $"{products.Count(x => x.Status == ProductStatus.Blocked)} bị chặn";

            return ServiceResult<StatisticSellerIndexDto>.Success(new StatisticSellerIndexDto
            {
                StatisticName = "Sản phẩm",
                Value = products.Count,
                Infomation = info
            });
        }

        #endregion

        // Tính tổng số tiền trong khoảng thời gian theo trạng thái escrow
        private decimal CalculateTotalAmount(IEnumerable<Escrow> escrows, DateTime startDate, DateTime endDate)
        {
            return escrows
                .Where(x => x.CreatedAt.Date >= startDate && x.CreatedAt.Date <= endDate)
                .Sum(x => x.Amount);
        }

        // Trả về ngày đầu và cuối tuần, có thể dùng offset tuần
        private (DateTime Start, DateTime End) GetWeekRange(int offsetWeek = 0)
        {
            var today = DateTime.Today;
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            var startOfWeek = today.AddDays(-diff).AddDays(7 * offsetWeek);
            var endOfWeek = startOfWeek.AddDays(6);
            return (startOfWeek, endOfWeek);
        }
        private (DateTime Start, DateTime End) GetMonthRange(int offsetMonth = 0)
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1).AddMonths(offsetMonth);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            return (firstDayOfMonth, lastDayOfMonth);
        }
        private (DateTime Start, DateTime End) GetYearRange(int offsetYear = 0)
        {
            var today = DateTime.Today;
            var firstDayOfYear = new DateTime(today.Year + offsetYear, 1, 1);
            var lastDayOfYear = new DateTime(today.Year + offsetYear, 12, 31);
            return (firstDayOfYear, lastDayOfYear);
        }
    }
}