using DPTS.Applications.Dtos;
using DPTS.Applications.Shareds;

namespace DPTS.Applications.Interfaces
{
    public interface IStatisticService
    {
        #region doanh thu
        Task<ServiceResult<StatisticSellerIndexDto>> SalesRevenueInDayAsync(string sellerId);
        Task<ServiceResult<StatisticSellerIndexDto>> SalesRevenueInWeekAsync(string sellerId);
        Task<ServiceResult<StatisticSellerIndexDto>> SalesRevenueInMonthAsync(string sellerId);
        Task<ServiceResult<StatisticSellerIndexDto>> SaleRevenueInYearAsync(string sellerId);
        #endregion


        Task<ServiceResult<StatisticSellerIndexDto>> RatingAsync(string sellerId);
        Task<ServiceResult<StatisticSellerIndexDto>> ProductOfSellerInWeekAsync(string sellerId);
    }
}
