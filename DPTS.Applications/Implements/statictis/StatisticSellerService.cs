using DPTS.Applications.Dtos.statisticals;
using DPTS.Applications.Interfaces;
using DPTS.Applications.Shareds;
using DPTS.Applications.Shareds.Interfaces;
using DPTS.Domains;
using Microsoft.Extensions.Logging;
using Org.BouncyCastle.Asn1.Esf;
using System.Linq;

namespace DPTS.Applications.Implements.statictis;

/// <summary>
/// Dịch vụ thống kê dành cho người bán.
/// </summary>
public class StatisticSellerService : IStatisticSellerService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<StatisticSellerService> _logger;

    public StatisticSellerService(IUnitOfWork unitOfWork, ILogger<StatisticSellerService> logger)
    {
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    #region Doanh thu

    /// <summary>
    /// Thống kê doanh thu tuần hiện tại và so sánh với tuần trước.
    /// </summary>
    public async Task<ServiceResult<BaseStatictiscalModel>> SalesRevenueAsync(string sellerId)
    {
        try
        {
            _logger.LogInformation("Thống kê doanh thu cho người bán {SellerId}", sellerId);

            var escrows = await _unitOfWork.Repository<Escrow>().GetManyAsync("SellerId", sellerId);
            if (!escrows.Any())
            {
                return ServiceResult<BaseStatictiscalModel>.Success(new()
                {
                    Name = "Doanh thu",
                    Quantity = 0,
                    Percentage = 0
                });
            }

            var (startThisWeek, endThisWeek) = GetWeekRange(0);
            var (startLastWeek, endLastWeek) = GetWeekRange(-1);

            decimal thisWeekTotal = CalculateTotalAmount(escrows, startThisWeek, endThisWeek);
            decimal lastWeekTotal = CalculateTotalAmount(escrows, startLastWeek, endLastWeek);

            int percentage = lastWeekTotal == 0
                ? (thisWeekTotal > 0 ? 100 : 0)
                : (int)((thisWeekTotal - lastWeekTotal) * 100 / lastWeekTotal);

            return ServiceResult<BaseStatictiscalModel>.Success(new()
            {
                Name = "Doanh thu",
                Quantity = thisWeekTotal,
                Percentage = percentage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi thống kê doanh thu cho người bán {SellerId}", sellerId);
            return ServiceResult<BaseStatictiscalModel>.Error("Đã xảy ra lỗi khi tính doanh thu.");
        }
    }

    #endregion

    #region Đơn hàng

    /// <summary>
    /// Thống kê số lượng đơn hàng đã hoàn thành trong tuần hiện tại và so sánh với tuần trước.
    /// </summary>
    public async Task<ServiceResult<BaseStatictiscalModel>> SoldOrdersAsync(string sellerId)
    {
        try
        {
            _logger.LogInformation("Thống kê đơn hàng đã bán cho người bán {SellerId}", sellerId);

            var (startThisWeek, endThisWeek) = GetWeekRange(0);
            var (startLastWeek, endLastWeek) = GetWeekRange(-1);

            var escrows = await _unitOfWork.Repository<Escrow>().GetManyAsync("UserId", sellerId);

            int countThisWeek = escrows.Count(x => x.Status == StatusEntity.Done && x.CreatedAt.Date >= startThisWeek && x.CreatedAt.Date <= endThisWeek);
            int countLastWeek = escrows.Count(x => x.Status == StatusEntity.Done && x.CreatedAt.Date >= startLastWeek && x.CreatedAt.Date <= endLastWeek);

            int percentage = countLastWeek == 0
                ? (countThisWeek > 0 ? 100 : 0)
                : (countThisWeek - countLastWeek) * 100 / countLastWeek;

            return ServiceResult<BaseStatictiscalModel>.Success(new()
            {
                Name = "Đơn hàng",
                Quantity = countThisWeek,
                Percentage = percentage
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi thống kê đơn hàng đã bán cho người bán {SellerId}", sellerId);
            return ServiceResult<BaseStatictiscalModel>.Error("Đã xảy ra lỗi khi thống kê đơn hàng.");
        }
    }

    #endregion

    #region Sản phẩm

    /// <summary>
    /// Thống kê số lượng sản phẩm đang hoạt động và đang chờ.
    /// </summary>
    public async Task<ServiceResult<BaseStatictiscalModel>> ProductStatisticAsync(string sellerId)
    {
        try
        {
            _logger.LogInformation("Thống kê sản phẩm cho người bán {SellerId}", sellerId);

            var products = await _unitOfWork.Repository<Product>().GetManyAsync("UserId", sellerId);
            if (!products.Any())
            {
                return ServiceResult<BaseStatictiscalModel>.Success(new()
                {
                    Name = "Sản phẩm",
                    MoreInfo = "Không có sản phẩm",
                    Quantity = 0
                });
            }

            int licensed = products.Count(x => x.IsCirculate == true);
            int pending = products.Count(x => x.IsCirculate == false);

            return ServiceResult<BaseStatictiscalModel>.Success(new()
            {
                Name = "Sản phẩm",
                Quantity = products.Count(),
                MoreInfo = $"{licensed} đang hoạt động, {pending} đang chờ"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi thống kê sản phẩm cho người bán {SellerId}", sellerId);
            return ServiceResult<BaseStatictiscalModel>.Error("Đã xảy ra lỗi khi thống kê sản phẩm.");
        }
    }

    #endregion

    #region Đánh giá

    /// <summary>
    /// Tính điểm đánh giá trung bình dựa trên tất cả sản phẩm của người bán.
    /// </summary>
    public async Task<ServiceResult<BaseStatictiscalModel>> RatedAsync(string sellerId)
    {
        try
        {
            _logger.LogInformation("Thống kê đánh giá sản phẩm cho người bán {SellerId}", sellerId);

            var products = await _unitOfWork.Repository<Product>().GetManyAsync("UserId", sellerId);
            if (!products.Any())
            {
                return ServiceResult<BaseStatictiscalModel>.Success(new()
                {
                    Name = "Đánh giá",
                    Percentage = 0,
                    MoreInfo = "Chưa ai đánh giá"
                });
            }

            int? totalRating = 0; int totalReviews = 0;

            foreach (var product in products)
            {
                var feedbacks = await _unitOfWork.Repository<ProductReview>().GetManyAsync("ProjectId", product.ProductId);
                totalRating += feedbacks.Sum(x => x.Rating);
                totalReviews += feedbacks.Count();
            }

            if (totalReviews == 0)
            {
                return ServiceResult<BaseStatictiscalModel>.Success(new()
                {
                    Name = "Đánh giá",
                    Percentage = 0,
                    MoreInfo = "Chưa ai đánh giá"
                });
            }

            decimal averageRating = (decimal)totalRating / totalReviews;

            return ServiceResult<BaseStatictiscalModel>.Success(new()
            {
                Name = "Đánh giá",
                Percentage = averageRating,
                MoreInfo = $"Số liệu dựa trên {totalReviews} đánh giá"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi thống kê đánh giá sản phẩm cho người bán {SellerId}", sellerId);
            return ServiceResult<BaseStatictiscalModel>.Error("Đã xảy ra lỗi khi lấy thông tin đánh giá.");
        }
    }

    #endregion

    #region Đơn hàng gần đây

    /// <summary>
    /// Lấy danh sách đơn hàng gần đây của người bán.
    /// </summary>
    public async Task<ServiceResult<IEnumerable<RecentOrderModel>>> RecentOrderAsync(string sellerId)
    {
        try
        {
            _logger.LogInformation("Lấy đơn hàng gần đây của người bán {SellerId}", sellerId);

            var escrows = await _unitOfWork.Repository<Escrow>().GetManyAsync("SellerId", sellerId);
            var recentEscrows = escrows
                .OrderByDescending(x => x.CreatedAt)
                .Take(10);

            var result = new List<RecentOrderModel>();

            foreach (var escrow in recentEscrows)
            {
                var orderResult = await GetOrderRecentAsync(escrow.OrderId, sellerId);
                if (orderResult.Status == StatusResult.Success && orderResult.Data != null)
                    result.Add(orderResult.Data);

                if (result.Count == 5) break;
            }

            return ServiceResult<IEnumerable<RecentOrderModel>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy đơn hàng gần đây cho người bán {SellerId}", sellerId);
            return ServiceResult<IEnumerable<RecentOrderModel>>.Error("Đã xảy ra lỗi khi lấy đơn hàng gần đây.");
        }
    }

    private async Task<ServiceResult<RecentOrderModel>> GetOrderRecentAsync(string orderId, string sellerId)
    {
        try
        {
            var order = await _unitOfWork.Repository<Order>().GetOneAsync("OrderId", orderId);
            if (order == null)
                return ServiceResult<RecentOrderModel>.Error("Không tìm thấy đơn hàng.");

            var buyer = await _unitOfWork.Repository<User>().GetOneAsync("UserId", order.BuyerId);
            if (buyer == null)
                return ServiceResult<RecentOrderModel>.Error("Không tìm thấy người mua.");

            var items = await _unitOfWork.Repository<OrderItem>().GetManyAsync("OrderId", orderId);

            int totalQuantity = 0;
            decimal totalAmount = 0;

            foreach (var item in items)
            {
                var product = await _unitOfWork.Repository<Product>().GetOneAsync("ProductId", item.ProductId);
                if (product?.SellerId != sellerId)
                    continue;

                totalQuantity += item.Quantity;
                totalAmount += item.Quantity * product.Price;
            }

            if (totalQuantity == 0)
                return ServiceResult<RecentOrderModel>.Error("Không có sản phẩm hợp lệ thuộc người bán.");

            return ServiceResult<RecentOrderModel>.Success(new RecentOrderModel
            {
                ImagePath = buyer.ImageUrl,
                BuyerName = buyer.Username,
                Amount = totalAmount,
                Text = $"Đã mua {totalQuantity} sản phẩm"
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi xử lý đơn hàng {OrderId}", orderId);
            return ServiceResult<RecentOrderModel>.Error("Đã xảy ra lỗi khi xử lý đơn hàng.");
        }
    }

    #endregion

    #region Liên hệ gần đây

    /// <summary>
    /// Thống kê danh sách người dùng đã nhắn tin gần đây đến người bán.
    /// </summary>
    public async Task<ServiceResult<IEnumerable<RecentContactModel>>> RecentContantAsync(string sellerId)
    {
        try
        {
            _logger.LogInformation("Lấy danh sách liên hệ gần đây với người bán {SellerId}", sellerId);

            var messages = await _unitOfWork.Repository<Message>().GetManyAsync(nameof(Message.ReceiverId), sellerId);
            if (!messages.Any())
                return ServiceResult<IEnumerable<RecentContactModel>>.Success(null!);

            var recentMessages = messages
                .OrderByDescending(x => x.CreatedAt)
                .DistinctBy(x => x.SenderId);

            var result = new List<RecentContactModel>();

            foreach (var message in recentMessages)
            {
                var sender = await _unitOfWork.Repository<User>().GetOneAsync("UserId", message.SenderId);
                if (sender == null) continue;

                result.Add(new RecentContactModel
                {
                    ImagePath = sender.ImageUrl,
                    Name = sender.FullName ?? "Không rõ",
                    SendAt = message.CreatedAt,
                    Text = message.Content
                });
            }

            return ServiceResult<IEnumerable<RecentContactModel>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi thống kê liên hệ gần đây.");
            return ServiceResult<IEnumerable<RecentContactModel>>.Error("Lỗi khi lấy liên hệ.");
        }
    }

    #endregion

    #region Bán chạy

    /// <summary>
    /// Thống kê các sản phẩm bán chạy nhất của người bán.
    /// </summary>
    public async Task<ServiceResult<IEnumerable<BaseStatictiscalModel>>> BestSellerAsync(string sellerId)
    {
        try
        {
            _logger.LogInformation("Thống kê sản phẩm bán chạy của người bán {SellerId}", sellerId);

           

            var escrows = await _unitOfWork.Repository<Escrow>().GetManyAsync(nameof(Escrow.SellerId), sellerId);
            if (!escrows.Any()) return ServiceResult<IEnumerable<BaseStatictiscalModel>>.Success(null!);
            var products = await _unitOfWork.Repository<Product>().GetManyAsync(nameof(Product.SellerId), sellerId);
            if (!products.Any() && escrows.Any()) return ServiceResult<IEnumerable<BaseStatictiscalModel>>.Error("");


            var result = new List<BaseStatictiscalModel>();

            foreach (var product in products)
            {
                int totalSold = 0;
                var images = await _unitOfWork.Repository<Image>().GetManyAsync(nameof(Image.ProductId), product.ProductId);

                foreach (var escrow in escrows.Where(x => x.Status == StatusEntity.Done))
                {
                    var items = await _unitOfWork.Repository<OrderItem>().GetManyAsync(nameof(OrderItem.OrderId), escrow.OrderId);
                    totalSold += items.Where(x => x.ProductId == product.ProductId).Sum(x => x.Quantity);
                }

                result.Add(new BaseStatictiscalModel
                {
                    Name = product.Title,
                    Quantity = totalSold,
                    ImageUrl = images.FirstOrDefault()?.ImagePath,
                    MoreInfo = $"{product.Price}đ"
                });
            }

            return ServiceResult<IEnumerable<BaseStatictiscalModel>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi thống kê sản phẩm bán chạy.");
            return ServiceResult<IEnumerable<BaseStatictiscalModel>>.Error("Lỗi khi lấy danh sách sản phẩm bán chạy.");
        }
    }

    #endregion

    #region Helpers

    private decimal CalculateTotalAmount(IEnumerable<Escrow> escrows, DateTime startDate, DateTime endDate)
    {
        return escrows
            .Where(x => x.Status == StatusEntity.Done && x.CreatedAt.Date >= startDate && x.CreatedAt.Date <= endDate)
            .Sum(x => x.Amount);
    }

    private (DateTime Start, DateTime End) GetWeekRange(int offsetWeek = 0)
    {
        var today = DateTime.Today;
        int diff = (7 + (today.DayOfWeek - DayOfWeek.Monday)) % 7;
        var startOfWeek = today.AddDays(-diff).AddDays(7 * offsetWeek);
        var endOfWeek = startOfWeek.AddDays(6);
        return (startOfWeek, endOfWeek);
    }

    #endregion
}
