using DPTS.Applications.Dtos;
using DPTS.Applications.Interfaces;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DPTS.Applications.Implements
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderService> _logger;
        public OrderService(ApplicationDbContext context, ILogger<OrderService> logger)
        {
            _context = context;
            _logger = logger;
        }

        #region Seller
        // Lấy danh sách đơn hàng của người bán theo sellerId
        public async Task<ServiceResult<IEnumerable<OrderIndexDto>>> GetOrdersBySellerId(string sellerId, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation("Fetching orders for seller {SellerId}, page {PageNumber}, size {PageSize}", sellerId, pageNumber, pageSize);

            // Truy vấn Escrows liên quan đến Seller + Order + OrderItems + Product
            var escrows = await _context.Escrows
                .Where(e => e.SellerId == sellerId)
                .Include(e => e.Order)
                    .ThenInclude(o => o.OrderItems.Where(oi => oi.Product.SellerId == sellerId))
                        .ThenInclude(oi => oi.Product)
                .OrderBy(e => e.CreatedAt)
                .ToListAsync();

            if (!escrows.Any())
            {
                return ServiceResult<IEnumerable<OrderIndexDto>>.Success(Enumerable.Empty<OrderIndexDto>());
            }

            // Lấy toàn bộ buyerId cần dùng (tránh gọi từng user)
            var buyerIds = escrows.Select(e => e.Order.BuyerId).Distinct().ToList();
            var buyers = await _context.Users
                .Where(u => buyerIds.Contains(u.UserId))
                .ToDictionaryAsync(u => u.UserId);

            var result = new List<OrderIndexDto>();
            foreach (var escrow in escrows)
            {
                var buyerName = buyers.TryGetValue(escrow.Order.BuyerId, out var buyer)
                    ? buyer.FullName ?? "N/A"
                    : "N/A";

                foreach (var item in escrow.Order.OrderItems)
                {
                    // Đảm bảo là của seller (phòng trường hợp Include không lọc hết)
                    if (item.Product.SellerId != sellerId) continue;

                    result.Add(new OrderIndexDto
                    {
                        OrderId = item.OrderId,
                        ProductionName = item.Product.ProductName,
                        BuyerName = buyerName,
                        Amount = item.TotalAmount,
                        Status = EnumHandle.HandleEscrowStatus(escrow.Status),
                    });
                }
            }

            return ServiceResult<IEnumerable<OrderIndexDto>>.Success(result.Skip((pageNumber-1)*pageSize).Take(pageSize));
        }
       
        // Đếm số đơn hàng đã bán trong ngày so với ngày trước
        public async Task<ServiceResult<StatisticSellerIndexDto>> SoldOrdersInDayAsync(string sellerId)
        {
            var escrows = await _context.Escrows
                .Where(x => x.Status == EscrowStatus.Done && x.SellerId == sellerId)
                .Include(x => x.Order)
                .ThenInclude(x => x.OrderItems)
                .ToListAsync();
            var today = DateTime.Today;
            var yesterday = today.AddDays(-1);
            var countThisDay = escrows.Count(x => x.CreatedAt.Date == today);
            var countLastDay = escrows.Count(x => x.CreatedAt.Date == yesterday);

            var percentage = countLastDay == 0
                ? (countThisDay > 0 ? 100 : 0)
                : (double)(countThisDay - countLastDay) * 100 / countLastDay;

            return ServiceResult<StatisticSellerIndexDto>.Success(new StatisticSellerIndexDto
            {
                StatisticName = "Đơn hàng ngày",
                Value = countThisDay,
                Infomation = $"{percentage} so với ngày trước"
            });
        }
        // Đếm số đơn hàng đã bán trong tuần so với tuần trước
        public async Task<ServiceResult<StatisticSellerIndexDto>> SoldOrdersInWeekAsync(string sellerId)
        {
            var escrows = await _context.Escrows
                .Where(x => x.Status == EscrowStatus.Done && x.SellerId == sellerId)
                .Include(x => x.Order)
                .ThenInclude(x => x.OrderItems)
                .ToListAsync();

            var (startThisWeek, endThisWeek) = GetWeekRange(0);
            var (startLastWeek, endLastWeek) = GetWeekRange(-1);

            int countThisWeek = escrows.Count(x => x.CreatedAt.Date >= startThisWeek && x.CreatedAt.Date <= endThisWeek);
            int countLastWeek = escrows.Count(x => x.CreatedAt.Date >= startLastWeek && x.CreatedAt.Date <= endLastWeek);

            double percentage = countLastWeek == 0
                ? (countThisWeek > 0 ? 100 : 0)
                : (double)(countThisWeek - countLastWeek) * 100 / countLastWeek;

            return ServiceResult<StatisticSellerIndexDto>.Success(new StatisticSellerIndexDto
            {
                StatisticName = "Đơn hàng",
                Value = countThisWeek,
                Infomation = $"{percentage} so với tuần trước"
            });
        }

        // Đếm số đơn hàng trong tháng so với tháng trước
        public async Task<ServiceResult<StatisticSellerIndexDto>> SoldOrdersInMonthAsync(string sellerId)
        {
            var escrows = await _context.Escrows
                .Where(x => x.Status == EscrowStatus.Done && x.SellerId == sellerId)
                .Include(x => x.Order)
                .ThenInclude(x => x.OrderItems)
                .ToListAsync();

            var (startThisMonth, endThisMonth) = GetMonthRange(0);
            var (startLastMonth, endLastMonth) = GetMonthRange(-1);

            int countThisMonth = escrows.Count(x => x.CreatedAt.Date >= startThisMonth && x.CreatedAt.Date <= endThisMonth);
            int countLastMonth = escrows.Count(x => x.CreatedAt.Date >= startLastMonth && x.CreatedAt.Date <= endLastMonth);

            double percentage = countLastMonth == 0
                ? (countThisMonth > 0 ? 100 : 0)
                : (double)(countThisMonth - countLastMonth) * 100 / countLastMonth;

            return ServiceResult<StatisticSellerIndexDto>.Success(new StatisticSellerIndexDto
            {
                StatisticName = "Đơn hàng",
                Value = countThisMonth,
                Infomation = $"{percentage} so với tháng trước"
            });
        }

        // Đếm số đơn hàng trong năm
        public async Task<ServiceResult<StatisticSellerIndexDto>> SoldOrdersInYearAsync(string sellerId)
        {
            var escrows = await _context.Escrows
                .Where(x => x.Status == EscrowStatus.Done && x.SellerId == sellerId)
                .Include(x => x.Order)
                .ThenInclude(x => x.OrderItems)
                .ToListAsync();

            var (startThisYear, endThisYear) = GetYearRange(0);
            var (startLastYear, endLastYear) = GetYearRange(-1);

            int countThisYear = escrows.Count(x => x.CreatedAt.Date >= startThisYear && x.CreatedAt.Date <= endThisYear);
            int countLastYear = escrows.Count(x => x.CreatedAt.Date >= startLastYear && x.CreatedAt.Date <= endLastYear);

            double percentage = countLastYear == 0
                ? (countThisYear > 0 ? 100 : 0)
                : (double)(countThisYear - countLastYear) * 100 / countLastYear;

            return ServiceResult<StatisticSellerIndexDto>.Success(new StatisticSellerIndexDto
            {
                StatisticName = "Đơn hàng",
                Value = countThisYear,
                Infomation = $"{percentage} so với năm trước"
            });
        }
        // Lấy các đơn hàng gần nhất của người bán
        public async Task<ServiceResult<IEnumerable<RecentOrderStatisticDto>>> RecentOrderAsync(string sellerId, int pageNumber = 2, int pageSize = 10)
        {
            var escrows = await _context.Escrows
                .Where(x => x.SellerId == sellerId)
                .Include(x => x.Order).ThenInclude(x => x.OrderItems).ThenInclude(x => x.Product)
                .Include(x => x.Order).ThenInclude(x => x.Buyer)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            var result = escrows.SelectMany(escrow => escrow.Order.OrderItems.Select(item => new RecentOrderStatisticDto
            {
                BuyerName = escrow.Order.Buyer.FullName ?? "N/A",
                BuyerImage = escrow.Order.Buyer.ImageUrl,
                OrderId = escrow.Order.OrderId,
                Information = item.Product.ProductName,
                Status = EnumHandle.HandleEscrowStatus(escrow.Status),
                Value = item.Product.Price
            })).ToList();

            return ServiceResult<IEnumerable<RecentOrderStatisticDto>>.Success(result.Skip((pageNumber - 1) * pageSize).Take(pageSize));
        }
        #endregion

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
