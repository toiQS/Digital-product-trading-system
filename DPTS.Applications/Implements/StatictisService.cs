using DPTS.Applications.Dtos;
using DPTS.Applications.Interfaces;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;

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

        // Thống kê doanh thu trong tuần so với tuần trước
        public async Task<ServiceResult<StatisticSellerIndexDto>> SalesRevenueInWeekAsync(string sellerId, int pageNumber = 2, int pageSize = 10)
        {

            var (startThisWeek, endThisWeek) = GetWeekRange(0);
            var (startLastWeek, endLastWeek) = GetWeekRange(-1);

            var escrows = await _context.Escrows
                .Where(x => x.SellerId == sellerId && x.Status == EscrowStatus.Done)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var salesRevenueThisWeek = CalculateTotalAmount(escrows, startThisWeek, endThisWeek);
            var salesRevenueLastWeek = CalculateTotalAmount(escrows, startLastWeek, endLastWeek);

            int percentage = salesRevenueLastWeek == 0
                ? (salesRevenueThisWeek > 0 ? 100 : 0)
                : (int)((salesRevenueThisWeek - salesRevenueLastWeek) * 100 / salesRevenueLastWeek);

            return ServiceResult<StatisticSellerIndexDto>.Success(new StatisticSellerIndexDto
            {
                StatisticName = "Doanh thu tuần",
                Value = salesRevenueThisWeek,
                Infomation = $"{percentage} so với tuần trước."
            });
        }

        // Đếm số đơn hàng đã bán trong tuần so với tuần trước
        public async Task<ServiceResult<StatisticSellerIndexDto>> SoldOrdersInWeekAsync(string sellerId, int pageNumber = 2, int pageSize = 10)
        {
            var escrows = await _context.Escrows
                .Where(x => x.Status == EscrowStatus.Done && x.SellerId == sellerId)
                .Include(x => x.Order)
                .ThenInclude(x => x.OrderItems)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var (startThisWeek, endThisWeek) = GetWeekRange(0);
            var (startLastWeek, endLastWeek) = GetWeekRange(-1);

            int countThisWeek = escrows.Count(x => x.CreatedAt.Date >= startThisWeek && x.CreatedAt.Date <= endThisWeek);
            int countLastWeek = escrows.Count(x => x.CreatedAt.Date >= startLastWeek && x.CreatedAt.Date <= endLastWeek);

            double percentage = countLastWeek == 0
                ? (countThisWeek > 0 ? 100 : 0)
                : (countThisWeek - countLastWeek) * 100 / countLastWeek;

            return ServiceResult<StatisticSellerIndexDto>.Success(new StatisticSellerIndexDto
            {
                StatisticName = "Đơn hàng",
                Value = countThisWeek,
                Infomation = $"{percentage} so với tuần trước"
            });
        }

        // Tính điểm đánh giá trung bình dựa trên toàn bộ sản phẩm của người bán
        public async Task<ServiceResult<StatisticSellerIndexDto>> RatingAsync(string sellerId, int pageSize = 10, int pageNumber = 1)
        {
            var products = await _context.Products
                .Where(x => x.SellerId == sellerId)
                .Include(x => x.Reviews)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var ratingSum = products.Sum(p => p.Reviews.Sum(r => r.Rating));
            var ratingCount = products.Sum(p => p.Reviews.Count);
            var averageRating = ratingCount > 0 ? ratingSum / ratingCount : 0;

            return ServiceResult<StatisticSellerIndexDto>.Success(new StatisticSellerIndexDto
            {
                StatisticName = "Đánh giá",
                Infomation = $"Dựa trên {ratingCount} đánh giá.",
                Value = averageRating
            });
        }

        // Thống kê số lượng sản phẩm và trạng thái của người bán trong tuần
        public async Task<ServiceResult<StatisticSellerIndexDto>> ProductOfSellerInWeekAsync(string sellerId, int pageSize = 10, int pageNumber = 2)
        {
            var products = await _context.Products
                .Where(x => x.SellerId == sellerId)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
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

        // Thống kê sản phẩm bán chạy của người bán
        public async Task<ServiceResult<IEnumerable<StatisticBestSellerIndexDto>>> BestSellAsync(string sellerId, int pageSize = 10, int pageNumber = 2)
        {
            var escrows = await _context.Escrows
                .Where(x => x.SellerId == sellerId && x.Status == EscrowStatus.Done)
                .Include(x => x.Order).ThenInclude(x => x.OrderItems)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var products = await _context.Products
                .Where(x => x.SellerId == sellerId)
                .Include(x => x.Images)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = new List<StatisticBestSellerIndexDto>();
            foreach (var product in products)
            {
                var quantity = escrows.SelectMany(e => e.Order.OrderItems)
                    .Where(oi => oi.ProductId == product.ProductId)
                    .Sum(oi => oi.Quantity);

                result.Add(new StatisticBestSellerIndexDto
                {
                    ProductName = product.ProductName,
                    Information = $"Đã bán {quantity}",
                    ProductImage = product.Images.OrderByDescending(x => x.CreateAt).FirstOrDefault()?.ImagePath ?? "default.png",
                    Value = product.Price
                });
            }

            return ServiceResult<IEnumerable<StatisticBestSellerIndexDto>>.Success(result);
        }

        // Lấy các tin nhắn gần nhất của người bán
        public async Task<ServiceResult<IEnumerable<RecentMessageIndexDto>>> RecentMessageAsync(string sellerId, int pageNumber = 2, int pageSize = 10)
        {
            var messages = await _context.Messages
                .Where(x => x.ReceiverId == sellerId)
                .OrderByDescending(x => x.CreatedAt)
                .DistinctBy(x => x.SenderId)
                .Include(x => x.Sender)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var result = messages.Select(x => new RecentMessageIndexDto
            {
                MessageId = x.MessageId,
                SenderName = x.Sender.FullName ?? "N/A",
                Content = x.Content,
                SendAt = x.CreatedAt.ToString("hh:mm - dd/MM/yyyy"),
                UserImage = x.Sender.ImageUrl
            });

            return ServiceResult<IEnumerable<RecentMessageIndexDto>>.Success(result);
        }

        // Lấy các đơn hàng gần nhất của người bán
        public async Task<ServiceResult<IEnumerable<RecentOrderStatisticDto>>> RecentOrderAsync(string sellerId, int pageNumber = 2, int pageSize = 10)
        {
            var escrows = await _context.Escrows
                .Where(x => x.SellerId == sellerId)
                .Include(x => x.Order).ThenInclude(x => x.OrderItems).ThenInclude(x => x.Product)
                .Include(x => x.Order).ThenInclude(x => x.Buyer)
                .OrderByDescending(x => x.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
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

            return ServiceResult<IEnumerable<RecentOrderStatisticDto>>.Success(result);
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
    }
}