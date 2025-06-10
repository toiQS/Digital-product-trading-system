using DPTS.Applications.Dtos;
using DPTS.Applications.Interfaces;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Implements
{
    public class SellerManagementService : ISellerManagementService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<SellerManagementService> _logger;
        public SellerManagementService(ApplicationDbContext context, ILogger<SellerManagementService> logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }
        #region Public methods
        #region Báo cáo
        // thống kê doanh thu trong 1 khoảng thời gian nhất định
        public async Task<ServiceResult<ReportIndexDto>> ReportRevenueDay(string sellerId)
        {
            return await BuildReportAsync(
                sellerId,
                () => GetEscrowByDateRange(DateTime.Today, DateTime.Today.AddDays(1)),
                () => GetEscrowByDateRange(DateTime.Today.AddDays(-1), DateTime.Today),
                "Doanh thu hôm nay",
                true);
        }

        public async Task<ServiceResult<ReportIndexDto>> ReportRevenueWeek(string sellerId, int offsetWeek = 0)
        {
            (var start, var end) = GetWeekRange(offsetWeek);
            return await BuildReportAsync(
                sellerId,
                () => GetEscrowByDateRange(start, end),
                () => GetEscrowByDateRange(start.AddDays(-7), end.AddDays(-7)),
                "Doanh thu tuần này",
                true);
        }

        public async Task<ServiceResult<ReportIndexDto>> ReportRevenueMonth(string sellerId, int offsetMonth = 0)
        {
            (var start, var end) = GetMonthRange(offsetMonth);
            return await BuildReportAsync(
                sellerId,
                () => GetEscrowByDateRange(start, end),
                () => GetEscrowByDateRange(start.AddMonths(-1), end.AddMonths(-1)),
                "Doanh thu tháng này",
                true);
        }

        public async Task<ServiceResult<ReportIndexDto>> ReportRevenueYear(string sellerId, int offsetYear = 0)
        {
            (var start, var end) = GetYearRange(offsetYear);
            return await BuildReportAsync(
                sellerId,
                () => GetEscrowByDateRange(start, end),
                () => GetEscrowByDateRange(start.AddYears(-1), end.AddYears(-1)),
                "Doanh thu năm này",
                true);
        }
        // thống kê số lượng đơn hàng trong 1 khoảng thời gian nhất định
        public async Task<ServiceResult<ReportIndexDto>> ReportOrderCountDay(string sellerId)
        {
            return await BuildReportAsync(
                sellerId,
                () => GetEscrowByDateRange(DateTime.Today, DateTime.Today.AddDays(1)),
                () => GetEscrowByDateRange(DateTime.Today.AddDays(-1), DateTime.Today),
                "Số lượng đơn hàng hôm nay",
                false);
        }

        public async Task<ServiceResult<ReportIndexDto>> ReportOrderCountWeek(string sellerId, int offsetWeek = 0)
        {
            (var start, var end) = GetWeekRange(offsetWeek);
            return await BuildReportAsync(
                sellerId,
                () => GetEscrowByDateRange(start, end),
                () => GetEscrowByDateRange(start.AddDays(-7), end.AddDays(-7)),
                "Số lượng đơn hàng tuần này",
                 false);
        }

        public async Task<ServiceResult<ReportIndexDto>> ReportOrderCountMonth(string sellerId, int offsetMonth = 0)
        {
            (var start, var end) = GetMonthRange(offsetMonth);
            return await BuildReportAsync(
                sellerId,
                () => GetEscrowByDateRange(start, end),
                () => GetEscrowByDateRange(start.AddMonths(-1), end.AddMonths(-1)),
                "Số lượng đơn hàng tháng này",
                false);
        }

        public async Task<ServiceResult<ReportIndexDto>> ReportOrderCountYear(string sellerId, int offsetYear = 0)
        {
            (var start, var end) = GetYearRange(offsetYear);
            return await BuildReportAsync(
                sellerId,
                () => GetEscrowByDateRange(start, end),
                () => GetEscrowByDateRange(start.AddYears(-1), end.AddYears(-1)),
                "Số lượng đơn hàng năm này",
                false);
        }

        // thống kê số lượng sản phẩm
        public async Task<ServiceResult<ReportIndexDto>> ReportQuantityProduct(string sellerId)
        {
            try
            {
                var products = await GetProductsWithManyCondition(sellerId: sellerId);
                var count = products.Count();
                var countAvailable = products.Count(x => x.Status == ProductStatus.Available);
                var countPending = products.Count(x => x.Status == ProductStatus.Pending);
                var countBlocked = products.Count(x => x.Status == ProductStatus.Blocked);

                return ServiceResult<ReportIndexDto>.Success(new ReportIndexDto
                {
                    StatisticName = "Sản phẩm",
                    Value = count,
                    Information = $"Đang bán: {countAvailable}, Chờ duyệt: {countPending}, Đã bị chặn: {countBlocked}."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "ReportQuantityProduct error for {sellerId}", sellerId);
                return ServiceResult<ReportIndexDto>.Error("Không thể lấy được số lượng sản phẩm.");
            }
        }

        // thống kê đánh giá cửa hàng
        public async Task<ServiceResult<ReportIndexDto>> ReportRatingAsync(string sellerId)
        {
            _logger.LogInformation("");
            try
            {
                var totalRating = (await GetProductsWithManyCondition(sellerId: sellerId)).Sum(x => x.Reviews.Sum(x => x.Rating));
                var count = (await GetProductsWithManyCondition(sellerId: sellerId)).Sum(x => x.Reviews.Count());
                return ServiceResult<ReportIndexDto>.Success(new ReportIndexDto()
                {
                    StatisticName = "Đánh giá",
                    Value = totalRating / count,
                    Information = $"Dựa trên {count} đánh giá."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError("");
                return ServiceResult<ReportIndexDto>.Error("Không báo cáo dược đánh giá.");
            }
        }
        #endregion

        #region Sản phẩm
        // sản phẩm bán chạy

        // lấy sản phẩm theo nhiều yêu cầu
        public async Task<ServiceResult<IEnumerable<ProductIndexDto>>> GetProductAsync(string sellerId = null!, string text = null!, string categoryId = null!, ProductStatus? status = ProductStatus.Unknown, int pageNumber = 1, int pageSize = 0)
        {
            _logger.LogInformation("GetProductAsync called with sellerId={sellerId}, text={text}, categoryId={categoryId}, status={status}", sellerId, text, categoryId, status);
            try
            {
                var products = await GetProductsWithManyCondition(text: text, categoryId: categoryId, sellerId: sellerId);
                if (status != ProductStatus.Unknown)
                {
                    products = products.Where(x => x.Status == status);
                }
                return ServiceResult<IEnumerable<ProductIndexDto>>.Success(products.Select(x => new ProductIndexDto()
                {
                    CategoryName = x.Category.CategoryName,
                    ProductName = x.ProductName,
                    Price = x.Price,
                    ProductId = x.ProductId,
                    QuantitySold = products
                                    .Where(p => p.OrderItems.Any(oi => oi.Order.Escrow.Status == EscrowStatus.Done))
                                    .Sum(p => p.OrderItems
                                        .Where(oi => oi.Order.Escrow.Status == EscrowStatus.Done)
                                        .Sum(oi => oi.Quantity)),
                    Status = EnumHandle.HandleProductStatus(x.Status)
                }).OrderBy(x => x.QuantitySold).Skip((pageNumber - 1) * pageSize).Take(pageSize));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "GetProductAsync error for sellerId={sellerId}, text={text}, categoryId={categoryId}, status={status}", sellerId, text, categoryId, status);
                return ServiceResult<IEnumerable<ProductIndexDto>>.Error("Không thể lấy được sản phẩm.");
            }
        }
        // thêm sản phẩm
        // chỉnh sửa thông tin sản phẩm
        // xóa sản phẩm
        #endregion

        #region Tin nhắn
        // liên hệ gần đây
        public async Task<ServiceResult<IEnumerable<MessageRecentIndexDto>>> GetRecentContactAsync(string sellerId, int pageNumber, int pageSize)
        {
            var messages = (await GetMessageAsync())
                .Where(x => x.ReceiverId == sellerId)
                .OrderByDescending(x => x.CreatedAt)
                .DistinctBy(x => x.SenderId)
                .ToList();

            return ServiceResult<IEnumerable<MessageRecentIndexDto>>.Success(messages.Select(x => new MessageRecentIndexDto
            {
                FullName = x.Sender.FullName,
                Content = x.Content,
                Image = x.Sender.ImageUrl ?? string.Empty,
                ReceiveAt = x.CreatedAt.ToString("dd/MM/yyyy HH:mm:ss")
            }));
        }
        #endregion

        #region Đơn hàng
        // danh sách đơn hàng đơn hàng theo nhiều yêu cầu
        public async Task<ServiceResult<IEnumerable<OrderIndexDto>>> GetOrdersBySellerId(string sellerId, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation("Fetching orders for seller with ID: {SellerId}", sellerId);
            var escrows = await GetEscrowsByManyConditionAsync(sellerId);
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
                    ProductName = x.Product.ProductName,
                    BuyerName = escrow.Order.Buyer.FullName!,
                    Price = x.Product.Price * x.Quantity,
                    Status = EnumHandle.HandleEscrowStatus(escrow.Status),
                    DateOrder = new DateOnly(escrow.CreatedAt.Year, escrow.CreatedAt.Month, escrow.CreatedAt.Day),
                    TimeOrder = new TimeOnly(escrow.CreatedAt.Hour, escrow.CreatedAt.Minute)

                }).ToList();
                orders.AddRange(orderitems);
            }
            return ServiceResult<IEnumerable<OrderIndexDto>>.Success(orders.OrderBy(x => x.OrderId).OrderByDescending(x => x.DateOrder).ThenByDescending(x => x.TimeOrder).Skip((pageNumber - 1) * pageSize).Take(pageSize));

        }
        #endregion

        #region Doanh thu
        // tồng doanh thu
        public async Task<ServiceResult<double>> GetTotalRevenueAsync(string sellerId)
        {
            try
            {
                var escrows = await GetEscrowsAsync();
                var totalRevenue = escrows
                    .Where(x => x.SellerId == sellerId && x.Status == EscrowStatus.Done)
                    .Sum(x => x.Amount);
                return ServiceResult<double>.Success(totalRevenue);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching total revenue for sellerId={sellerId}", sellerId);
                return ServiceResult<double>.Error("Không thể lấy được doanh thu.");
            }
        }
        // doanh thu tháng 
        // tổng số đơn hàng
        public async Task<ServiceResult<double>> GetTotalOrderCountAsync(string sellerId)
        {
            try
            {
                var escrows = await GetEscrowsAsync();
                var totalOrders = escrows
                    .Where(x => x.SellerId == sellerId && x.Status == EscrowStatus.Done)
                    .Count();
                return ServiceResult<double>.Success(totalOrders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching total order count for sellerId={sellerId}", sellerId);
                return ServiceResult<double>.Error("Không thể lấy được số lượng đơn hàng.");
            }
        }
        // top 3 sản phẩm bán chạy
        public async Task<ServiceResult<IEnumerable<ProductBestSaleIndexDto>>> GetTop3BestSellingProductsAsync(string sellerId)
        {
            try
            {
                var products = await GetProductsWithManyCondition(sellerId: sellerId);
                var bestSellingProducts = products
                    .Select(x => new ProductBestSaleIndexDto
                    {
                        ProductName = x.ProductName,
                        QuantitySold = x.OrderItems.Sum(oi => oi.Quantity),
                        Price = x.Price,
                        ImagePath = x.Images.FirstOrDefault()?.ImagePath ?? string.Empty
                    })
                    .OrderByDescending(x => x.QuantitySold)
                    .Take(3);
                return ServiceResult<IEnumerable<ProductBestSaleIndexDto>>.Success(bestSellingProducts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching top 3 best selling products for sellerId={sellerId}", sellerId);
                return ServiceResult<IEnumerable<ProductBestSaleIndexDto>>.Error("Không thể lấy được sản phẩm bán chạy.");
            }
        }
        #endregion

        #region khiếu nại

        // danh sách khiếu nại theo tình trạng
        public async Task<ServiceResult<IEnumerable<ComplaintIndexDto>>> GetComplaintsByManyCondition(string sellerId = null!, string text = null!, ComplaintStatus? status = null)
        {
            try
            {
                var complaints = await GetComplaintsAsync();
                if (!string.IsNullOrEmpty(sellerId))
                {
                    complaints = complaints.Where(x => x.Order.Escrow.SellerId == sellerId);
                }
                if (!string.IsNullOrEmpty(text))
                {
                    complaints = complaints.Where(x => x.Product.ProductName.Contains(text, StringComparison.OrdinalIgnoreCase));
                }
                if (status.HasValue)
                {
                    complaints = complaints.Where(x => x.Status == status);
                }

                return ServiceResult<IEnumerable<ComplaintIndexDto>>.Success(complaints.Select(x => new ComplaintIndexDto
                {
                    ComplaintId = x.ComplaintId,
                    ImageBuyer = x.User.ImageUrl ?? string.Empty,
                    ComplaintName = x.Title,
                    Description = x.Description,
                    BuyerName = x.User.FullName ?? string.Empty,
                    OrderId = x.Order.OrderId,
                    Status = EnumHandle.HandleComplaintStatus(x.Status),
                    ComplaintAt = x.CreatedAt,
                }));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching complaints for sellerId={sellerId}, text={text}, status={status}", sellerId, text, status);
                return ServiceResult<IEnumerable<ComplaintIndexDto>>.Error("Không thể lấy được khiếu nại.");
            }
        }
        #endregion

        #endregion

        #region Private methods

        private async Task<IEnumerable<Escrow>> GetEscrowByDateRange(DateTime startDate, DateTime endDate)
        {
            return (await GetEscrowsAsync())
                .Where(x => x.CreatedAt >= startDate && x.CreatedAt <= endDate)
                .ToList();
        }
        private (DateTime Start, DateTime End) GetWeekRange(int offsetWeek = 0)
        {
            DateTime today = DateTime.Today;
            int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
            DateTime startOfWeek = today.AddDays(-diff).AddDays(7 * offsetWeek);
            DateTime endOfWeek = startOfWeek.AddDays(6);
            return (startOfWeek, endOfWeek);
        }
        private (DateTime Start, DateTime End) GetMonthRange(int offsetMonth = 0)
        {
            DateTime today = DateTime.Today;
            DateTime firstDayOfMonth = new DateTime(today.Year, today.Month, 1).AddMonths(offsetMonth);
            DateTime lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            return (firstDayOfMonth, lastDayOfMonth);
        }
        private (DateTime Start, DateTime End) GetYearRange(int offsetYear = 0)
        {
            DateTime today = DateTime.Today;
            DateTime firstDayOfYear = new(today.Year + offsetYear, 1, 1);
            DateTime lastDayOfYear = new(today.Year + offsetYear, 12, 31);
            return (firstDayOfYear, lastDayOfYear);
        }
        private async Task<IEnumerable<Product>> GetProductsWithManyCondition(string? text = null, string? categoryId = null, string? sellerId = null)
        {
            IQueryable<Product> query = _context.Products
                .Include(x => x.Seller)
                .Include(x => x.Category)
                .Include(x => x.Images)
                .Include(x => x.Reviews)
                .Include(x => x.OrderItems)
                    .ThenInclude(x => x.Order)
                        .ThenInclude(x => x.Escrow)
                .AsNoTracking();


            if (!string.IsNullOrEmpty(text))
            {
                query = query.Where(x =>
                    x.ProductName.Contains(text, StringComparison.OrdinalIgnoreCase) ||
                    (x.Description != null && x.Description.Contains(text, StringComparison.OrdinalIgnoreCase)));
            }

            if (!string.IsNullOrEmpty(categoryId))
            {
                query = query.Where(x => x.CategoryId == categoryId);
            }

            if (!string.IsNullOrEmpty(sellerId))
            {
                query = query.Where(x => x.SellerId == sellerId);
            }

            return await query.ToListAsync();
        }

        private async Task<ServiceResult<ReportIndexDto>> BuildReportAsync(string sellerId, Func<Task<IEnumerable<Escrow>>> currentFunc, Func<Task<IEnumerable<Escrow>>> previousFunc, string title, bool isRevenue)
        {
            try
            {
                var current = (await currentFunc()).ToList();
                var previous = (await previousFunc()).ToList();

                double currentValue = isRevenue ? current.Sum(x => x.Amount) : current.Count;
                double previousValue = isRevenue ? previous.Sum(x => x.Amount) : previous.Count;
                double percentageChange = (currentValue - previousValue) / (previousValue == 0 ? 1 : previousValue) * 100;

                var unit = isRevenue ? "VNĐ" : "đơn hàng";

                return ServiceResult<ReportIndexDto>.Success(new ReportIndexDto
                {
                    StatisticName = title,
                    Value = currentValue,
                    Information = $"{currentValue:N0} {unit}, {percentageChange:F2}% so với kỳ trước."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error building report for {title}, sellerId={sellerId}", title, sellerId);
                return ServiceResult<ReportIndexDto>.Error($"Không thể tạo báo cáo: {title}.");
            }
        }
        private async Task<IEnumerable<Message>> GetMessageAsync()
        {
            return await _context.Messages
                 .Include(x => x.Sender)
                 .Include(x => x.Receiver)
                 .Include(x => x.Order)
                 .ToListAsync();
        }
        private async Task<IEnumerable<Escrow>> GetEscrowsAsync()
        {
            return await _context.Escrows
                .Include(e => e.Order)
                    .ThenInclude(x => x.Buyer) // thông tin người mua
                .Include(e => e.Order)
                    .ThenInclude(x => x.OrderItems)
                        .ThenInclude(x => x.Product)
                .Include(e => e.Seller) // thông tin người bán
                .Where(x => x.Order.IsPaied == true) // chỉ lấy những đơn hàng đã thanh toán
                .AsNoTracking()
                .ToArrayAsync();
        }
        private async Task<IEnumerable<Escrow>> GetEscrowsByManyConditionAsync(string sellerId = null!, string text = null!, EscrowStatus? status = null)
        {
            var escrows = await GetEscrowsAsync();
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

        private async Task<IEnumerable<Complaint>> GetComplaintsAsync()
        {
            return await _context.Complications
                .Include(x => x.Product)
                .Include(x => x.User)
                .Include(x => x.Order)
                    .ThenInclude(x => x.Escrow)
                        .ThenInclude(x => x.Seller)
                .AsNoTracking()
                .ToListAsync();
        }
        #endregion
    }
}
