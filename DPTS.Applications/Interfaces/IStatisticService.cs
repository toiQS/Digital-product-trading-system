using DPTS.Applications.Dtos;
using DPTS.Applications.Shareds;

namespace DPTS.Applications.Interfaces
{
    public interface IStatisticService
    {
        Task<ServiceResult<StatisticSellerIndexDto>> SalesRevenueInWeekAsync(string sellerId, int pageNumber = 2, int pageSize = 10);
        Task<ServiceResult<StatisticSellerIndexDto>> SoldOrdersInWeekAsync(string sellerId, int pageNumber = 2, int pageSize = 10);
        Task<ServiceResult<StatisticSellerIndexDto>> RatingAsync(string sellerId, int pageSize = 10, int pageNumber = 1);
        Task<ServiceResult<StatisticSellerIndexDto>> ProductOfSellerInWeekAsync(string sellerId, int pageSize = 10, int pageNumber = 2);
        Task<ServiceResult<IEnumerable<StatisticBestSellerIndexDto>>> BestSellAsync(string sellerId, int pageSize = 10, int pageNumber = 2);
        Task<ServiceResult<IEnumerable<RecentMessageIndexDto>>> RecentMessageAsync(string sellerId, int pageNumber = 2, int pageSize = 10);
        Task<ServiceResult<IEnumerable<RecentOrderStatisticDto>>> RecentOrderAsync(string sellerId, int pageNumber = 2, int pageSize = 10);
    }
}
