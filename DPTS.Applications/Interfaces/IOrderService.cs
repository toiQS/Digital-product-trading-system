using DPTS.Applications.Dtos;
using DPTS.Applications.Shareds;

namespace DPTS.Applications.Interfaces
{
    public interface IOrderService
    {
        Task<ServiceResult<IEnumerable<OrderIndexDto>>> GetOrdersBySellerId(string sellerId, int pageNumber = 1, int pageSize = 10);
        Task<ServiceResult<StatisticSellerIndexDto>> SoldOrdersInDayAsync(string sellerId);
        Task<ServiceResult<StatisticSellerIndexDto>> SoldOrdersInWeekAsync(string sellerId);
        Task<ServiceResult<StatisticSellerIndexDto>> SoldOrdersInMonthAsync(string sellerId);
        Task<ServiceResult<StatisticSellerIndexDto>> SoldOrdersInYearAsync(string sellerId);
        Task<ServiceResult<IEnumerable<RecentOrderStatisticDto>>> RecentOrderAsync(string sellerId, int pageNumber = 2, int pageSize = 10);

    }
}
