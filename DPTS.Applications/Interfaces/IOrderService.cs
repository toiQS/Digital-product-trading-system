using DPTS.Applications.Dtos;
using DPTS.Applications.Shareds;
using DPTS.Domains;

namespace DPTS.Applications.Interfaces
{
    public interface IOrderService
    {
        #region Seller
        Task<ServiceResult<IEnumerable<OrderIndexDto>>> GetOrdersBySellerId(string sellerId, int pageNumber = 1, int pageSize = 10);
        Task<ServiceResult<StatisticSellerIndexDto>> GetSoldOrderInRangeTimeAsync(string sellerId, bool isDay, bool isWeek, bool isMonth, bool isYear);
        Task<ServiceResult<IEnumerable<OrderIndexDto>>> GetOrderWithManyConditionAsync(DateTime from, DateTime to, int pageNumber, int pageSize, string sellerId, string text, EscrowStatus? status = null);
        #endregion
        #region Buyer
        #endregion
    }
}
