using DPTS.Applications.Dtos.statisticals;
using DPTS.Applications.Shareds;

namespace DPTS.Applications.Interfaces
{
    public interface IStatisticSellerService
    {
        Task<ServiceResult<BaseStatictiscalModel>> SalesRevenueAsync(string sellerId);
        Task<ServiceResult<BaseStatictiscalModel>> SoldOrdersAsync(string sellerId);
        Task<ServiceResult<BaseStatictiscalModel>> ProductStatisticAsync(string sellerId);
        Task<ServiceResult<BaseStatictiscalModel>> RatedAsync(string sellerId);
        Task<ServiceResult<IEnumerable<RecentOrderModel>>> RecentOrderAsync(string sellerId);
        Task<ServiceResult<IEnumerable<RecentContactModel>>> RecentContantAsync(string sellerId);
        Task<ServiceResult<IEnumerable<BaseStatictiscalModel>>> BestSellerAsync(string sellerId);
    }
}
