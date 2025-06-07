using DPTS.Applications.Dtos;
using DPTS.Applications.Interfaces;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Logging;
using Microsoft.VisualBasic;
using System.Data;
using System.Reflection.Metadata.Ecma335;
using System.Threading.Tasks;

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
        public async Task<ServiceResult<IEnumerable<OrderIndexDto>>> GetOrdersBySellerId(string sellerId, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation("Fetching orders for seller with ID: {SellerId}", sellerId);
            var escrows = GetEscrowsByManyConditionAsync(await GetEscrowsAsync(), sellerId);
            if (escrows == null || !escrows.Any())
            {
                return ServiceResult<IEnumerable<OrderIndexDto>>.Error("Không có đơn hàng nào.");
            }
            var orders = new List<OrderIndexDto>();
            foreach (var escrow in escrows)
            {
                var orderitems = escrow.Order.OrderItems.Select(x => new OrderIndexDto()
                {
                    OrderId = escrow.Order.OrderId,
                    ProductionName = x.Product.ProductName,
                    BuyerName = escrow.Order.Buyer.FullName!,
                    Amount = x.Product.Price * x.Quantity,
                    Status = EnumHandle.HandleEscrowStatus(escrow.Status),
                    CreatedAt = escrow.CreatedAt,
                }).ToList();
                orders.AddRange(orderitems);
            }
            return ServiceResult<IEnumerable<OrderIndexDto>>.Success(orders.OrderBy(x => x.OrderId).OrderByDescending(x => x.CreatedAt).Skip((pageNumber - 1) * pageSize).Take(pageSize));

        }
        public async Task<ServiceResult<StatisticSellerIndexDto>> GetSoldOrderInRangeTimeAsync(string sellerId, bool isDay, bool isWeek, bool isMonth, bool isYear)
        {
            var percentage = 0.0;
            
            switch (isDay, isWeek, isMonth, isYear)
            {
                case (true, false, false, false):
                    {
                        var totalOrdersToday = CalculateQuantityOrderIsSold(GetEscrowsByManyConditionAsync(await GetEscrowsAsync(), sellerId, status: EscrowStatus.Done), DateTime.Today, DateTime.Today);
                        var totalOrdersYesterday = CalculateQuantityOrderIsSold(GetEscrowsByManyConditionAsync(await GetEscrowsAsync(), sellerId, status: EscrowStatus.Done), DateTime.Today.AddDays(-1), DateTime.Today.AddDays(-1));
                        percentage = totalOrdersYesterday == 0 ? (totalOrdersToday > 0 ? 100 : 0) : (double)((totalOrdersToday - totalOrdersYesterday) * 100 / totalOrdersYesterday);
                        return ServiceResult<StatisticSellerIndexDto>.Success(new StatisticSellerIndexDto
                        {
                            StatisticName = "Đơn hàng",
                            Value = totalOrdersToday,
                            Infomation = $"{percentage} so với ngày trước."
                        });
                    }
                case (false, true, false, false):
                    {
                        var (startThisWeek, endThisWeek) = GetWeekRange(0);
                        var (startLastWeek, endLastWeek) = GetWeekRange(-1);
                        var totalOrdersWeek = CalculateQuantityOrderIsSold(GetEscrowsByManyConditionAsync(await GetEscrowsAsync(), sellerId, status: EscrowStatus.Done), startThisWeek, endThisWeek);
                        var totalOrdersYesterday = CalculateQuantityOrderIsSold(GetEscrowsByManyConditionAsync(await GetEscrowsAsync(), sellerId, status: EscrowStatus.Done), startLastWeek, endLastWeek);
                        percentage = totalOrdersYesterday == 0 ? (totalOrdersWeek > 0 ? 100 : 0) : (double)((totalOrdersWeek - totalOrdersYesterday) * 100 / totalOrdersYesterday);
                        return ServiceResult<StatisticSellerIndexDto>.Success(new StatisticSellerIndexDto
                        {
                            StatisticName = "Đơn hàng",
                            Value = totalOrdersWeek,
                            Infomation = $"{percentage} so với tuần trước."
                        });
                    }
                case (false, false, true, false):
                    {
                        var (startThisMonth, endThisMonth) = GetMonthRange(0);
                        var (startLastMonth, endLastMonth) = GetMonthRange(-1);
                        var totalOrdersMonth = CalculateQuantityOrderIsSold(GetEscrowsByManyConditionAsync(await GetEscrowsAsync(), sellerId, status: EscrowStatus.Done), startThisMonth, endThisMonth);
                        var totalOrdersYesterday = CalculateQuantityOrderIsSold(GetEscrowsByManyConditionAsync(await GetEscrowsAsync(), sellerId, status: EscrowStatus.Done), startLastMonth, endLastMonth);
                        percentage = totalOrdersYesterday == 0 ? (totalOrdersMonth > 0 ? 100 : 0) : (double)((totalOrdersMonth - totalOrdersYesterday) * 100 / totalOrdersYesterday);
                        return ServiceResult<StatisticSellerIndexDto>.Success(new StatisticSellerIndexDto
                        {
                            StatisticName = "Đơn hàng",
                            Value = totalOrdersMonth,
                            Infomation = $"{percentage} so với tháng trước."
                        });
                    }
                case (false, false, false, true):
                    {
                        var (startThisYear, endThisYear) = GetYearRange(0);
                        var (startLastYear, endLastYear) = GetYearRange(-1);
                        var totalOrdersYear = CalculateQuantityOrderIsSold(GetEscrowsByManyConditionAsync(await GetEscrowsAsync(), sellerId, status: EscrowStatus.Done), startThisYear, endThisYear);
                        var totalOrdersYesterday = CalculateQuantityOrderIsSold(GetEscrowsByManyConditionAsync(await GetEscrowsAsync(), sellerId, status: EscrowStatus.Done), startLastYear, endLastYear);
                        percentage = totalOrdersYesterday == 0 ? (totalOrdersYear > 0 ? 100 : 0) : (double)((totalOrdersYear - totalOrdersYesterday) * 100 / totalOrdersYesterday);
                        return ServiceResult<StatisticSellerIndexDto>.Success(new StatisticSellerIndexDto
                        {
                            StatisticName = "Đơn hàng",
                            Value = totalOrdersYear,
                            Infomation = $"{percentage} so với năm trước."
                        });
                    }
                default:
                    return ServiceResult<StatisticSellerIndexDto>.Error("Vui lòng chọn một khoảng thời gian hợp lệ.");
            }
        }
        public async Task<ServiceResult<IEnumerable<OrderIndexDto>>> GetOrderWithManyConditionAsync( DateTime from, DateTime to, int pageNumber, int pageSize , string sellerId, string text, EscrowStatus? status = null)
        {
            _logger.LogInformation("Fetching orders with conditions: SellerId={SellerId}, Text={Text}, Status={Status}, From={From}, To={To}", sellerId, text, status, from, to);
            try
            {
                var escrows = GetEscrowsInRangeTime(GetEscrowsByManyConditionAsync(await GetEscrowsAsync(), sellerId, text, status), from, to);
                var orders = new List<OrderIndexDto>();
                foreach(var escrow in escrows)
                {
                    var orderitems = escrow.Order.OrderItems.Select(x => new OrderIndexDto()
                    {
                        OrderId = escrow.Order.OrderId,
                        ProductionName = x.Product.ProductName,
                        BuyerName = escrow.Order.Buyer.FullName!,
                        Amount = x.Product.Price * x.Quantity,
                        Status = EnumHandle.HandleEscrowStatus(escrow.Status),
                        CreatedAt = escrow.CreatedAt,
                    }).ToList();
                    orders.AddRange(orderitems);
                }
                return ServiceResult<IEnumerable<OrderIndexDto>>.Success(orders.OrderBy(x => x.OrderId).OrderByDescending(x => x.CreatedAt).Skip((pageNumber-1)*pageSize).Take(pageSize));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching orders with conditions: SellerId={SellerId}, Text={Text}, Status={Status}, From={From}, To={To}", sellerId, text, status, from, to);
                return ServiceResult<IEnumerable<OrderIndexDto>>.Error("Lỗi khi lấy đơn hàng với điều kiện đã cho.");
            }

        }
        #endregion
        #region Buyer
        #endregion
        #region Private Methods
        // Trả về ngày đầu và cuối tuần, có thể dùng offset tuần
        private (DateTime Start, DateTime End) GetWeekRange(int offsetWeek = 0)
        {
            var today = DateTime.Today;
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            var startOfWeek = today.AddDays(-diff).AddDays(7 * offsetWeek);
            var endOfWeek = startOfWeek.AddDays(6);
            return (startOfWeek, endOfWeek);
        }
        // Trả về ngày đầu và cuối tháng, có thể dùng offset tháng
        private (DateTime Start, DateTime End) GetMonthRange(int offsetMonth = 0)
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1).AddMonths(offsetMonth);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            return (firstDayOfMonth, lastDayOfMonth);
        }
        // Trả về ngày đầu và cuối năm, có thể dùng offset năm
        private (DateTime Start, DateTime End) GetYearRange(int offsetYear = 0)
        {
            var today = DateTime.Today;
            var firstDayOfYear = new DateTime(today.Year + offsetYear, 1, 1);
            var lastDayOfYear = new DateTime(today.Year + offsetYear, 12, 31);
            return (firstDayOfYear, lastDayOfYear);
        }
       
        private async Task<IEnumerable<Escrow>> GetEscrowsAsync()
        {
            return await _context.Escrows
                .Include(e => e.Order)
                    .ThenInclude(x => x.Buyer) // thông tin người mua
                .Include(e => e.Order)
                    .ThenInclude(x => x.OrderItems)
                        .ThenInclude(x => x.Product)
                .Include(e => e.User) // thông tin người bán
                .AsNoTracking()
                .ToArrayAsync();
        }
        private IEnumerable<Escrow> GetEscrowsByManyConditionAsync(IEnumerable<Escrow> escrows, string sellerId = null!, string text = null!, EscrowStatus? status = null)
        {
            switch (status)
            {
                default:
                    return escrows;
                case var _ when string.IsNullOrEmpty(sellerId) && string.IsNullOrEmpty(text) && status == EscrowStatus.Unknown:
                    return escrows.Where(x => x.Status == EscrowStatus.Unknown);
                case var _ when string.IsNullOrEmpty(sellerId) && string.IsNullOrEmpty(text) && status == EscrowStatus.Done:
                    return escrows.Where(x => x.Status != EscrowStatus.Done);
                case var _ when string.IsNullOrEmpty(sellerId) && string.IsNullOrEmpty(text) && status != EscrowStatus.WaitingComfirm:
                    return escrows.Where(x => x.Status != EscrowStatus.WaitingComfirm);
                case var _ when string.IsNullOrEmpty(sellerId) && string.IsNullOrEmpty(text) && status != EscrowStatus.Comfirmed:
                    return escrows.Where(x => x.Status != EscrowStatus.Comfirmed);
                case var _ when string.IsNullOrEmpty(sellerId) && string.IsNullOrEmpty(text) && status != EscrowStatus.Canceled:
                    return escrows.Where(x => x.Status != EscrowStatus.Canceled);

                //when sellerId is not null or empty
                case var _ when !string.IsNullOrEmpty(sellerId) && string.IsNullOrEmpty(text) && status == EscrowStatus.Unknown:
                    return escrows;
                case var _ when string.IsNullOrEmpty(sellerId) && string.IsNullOrEmpty(text) && status != EscrowStatus.Done:
                    return escrows.Where(x => x.Status != EscrowStatus.Done && x.SellerId == sellerId);
                case var _ when string.IsNullOrEmpty(sellerId) && string.IsNullOrEmpty(text) && status != EscrowStatus.WaitingComfirm:
                    return escrows.Where(x => x.Status != EscrowStatus.WaitingComfirm && x.SellerId == sellerId);
                case var _ when string.IsNullOrEmpty(sellerId) && string.IsNullOrEmpty(text) && status != EscrowStatus.Comfirmed:
                    return escrows.Where(x => x.Status != EscrowStatus.Comfirmed && x.SellerId == sellerId);
                case var _ when string.IsNullOrEmpty(sellerId) && string.IsNullOrEmpty(text) && status != EscrowStatus.Canceled:
                    return escrows.Where(x => x.Status != EscrowStatus.Canceled && x.SellerId == sellerId);

                //when text is not null or empty
                case var _ when string.IsNullOrEmpty(sellerId) && string.IsNullOrEmpty(text) && status == EscrowStatus.Unknown:
                    return escrows.Where(x => x.Status == EscrowStatus.Unknown && x.Order.Buyer.FullName!.ToLower().Contains(text.ToLower()));
                case var _ when string.IsNullOrEmpty(sellerId) && string.IsNullOrEmpty(text) && status == EscrowStatus.Done:
                    return escrows.Where(x => x.Status != EscrowStatus.Done && x.Order.Buyer.FullName!.ToLower().Contains(text.ToLower()));
                case var _ when string.IsNullOrEmpty(sellerId) && string.IsNullOrEmpty(text) && status != EscrowStatus.WaitingComfirm:
                    return escrows.Where(x => x.Status != EscrowStatus.WaitingComfirm && x.Order.Buyer.FullName!.ToLower().Contains(text.ToLower()));
                case var _ when string.IsNullOrEmpty(sellerId) && string.IsNullOrEmpty(text) && status != EscrowStatus.Comfirmed:
                    return escrows.Where(x => x.Status != EscrowStatus.Comfirmed && x.Order.Buyer.FullName!.ToLower().Contains(text.ToLower()));
                case var _ when string.IsNullOrEmpty(sellerId) && string.IsNullOrEmpty(text) && status != EscrowStatus.Canceled:
                    return escrows.Where(x => x.Status != EscrowStatus.Canceled && x.Order.Buyer.FullName!.ToLower().Contains(text.ToLower()));

                // when sellerId and text is not null or empty
                case var _ when string.IsNullOrEmpty(sellerId) && string.IsNullOrEmpty(text) && status == EscrowStatus.Unknown:
                    return escrows.Where(x => x.Status == EscrowStatus.Unknown && x.Order.Buyer.FullName!.ToLower().Contains(text.ToLower()) && x.SellerId == sellerId);
                case var _ when string.IsNullOrEmpty(sellerId) && string.IsNullOrEmpty(text) && status == EscrowStatus.Done:
                    return escrows.Where(x => x.Status != EscrowStatus.Done && x.Order.Buyer.FullName!.ToLower().Contains(text.ToLower()) && x.SellerId == sellerId);
                case var _ when string.IsNullOrEmpty(sellerId) && string.IsNullOrEmpty(text) && status != EscrowStatus.WaitingComfirm:
                    return escrows.Where(x => x.Status != EscrowStatus.WaitingComfirm && x.Order.Buyer.FullName!.ToLower().Contains(text.ToLower()) && x.SellerId == sellerId);
                case var _ when string.IsNullOrEmpty(sellerId) && string.IsNullOrEmpty(text) && status != EscrowStatus.Comfirmed:
                    return escrows.Where(x => x.Status != EscrowStatus.Comfirmed && x.Order.Buyer.FullName!.ToLower().Contains(text.ToLower()) && x.SellerId == sellerId);
                case var _ when string.IsNullOrEmpty(sellerId) && string.IsNullOrEmpty(text) && status != EscrowStatus.Canceled:
                    return escrows.Where(x => x.Status != EscrowStatus.Canceled && x.Order.Buyer.FullName!.ToLower().Contains(text.ToLower()) && x.SellerId == sellerId);
            }
        }
        private IEnumerable<Escrow> GetEscrowsInRangeTime(IEnumerable<Escrow> escrows, DateTime start, DateTime end)
        {
            return escrows.Where(x => x.CreatedAt >= start && x.CreatedAt <= end);
        }
        private decimal CalculateQuantityOrderIsSold(IEnumerable<Escrow> escrows, DateTime startDate, DateTime endDate)
        {
            return escrows
                .Where(x => x.CreatedAt.Date >= startDate && x.CreatedAt.Date <= endDate).Count(); 
        }
        
        #endregion
    }
}
