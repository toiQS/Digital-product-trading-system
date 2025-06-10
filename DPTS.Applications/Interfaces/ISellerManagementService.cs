using DPTS.Applications.Dtos;
using DPTS.Applications.Shareds;
using DPTS.Domains;

namespace DPTS.Applications.Interfaces
{
    public interface ISellerManagementService
    {
        Task<ServiceResult<IEnumerable<ComplaintIndexDto>>> GetComplaintsByManyCondition(string sellerId = null, string text = null, ComplaintStatus? status = null);
        Task<ServiceResult<IEnumerable<OrderIndexDto>>> GetOrdersBySellerId(string sellerId, int pageNumber = 1, int pageSize = 10);
        Task<ServiceResult<IEnumerable<ProductIndexDto>>> GetProductAsync(string sellerId = null, string text = null, string categoryId = null, ProductStatus? status = ProductStatus.Unknown, int pageNumber = 1, int pageSize = 0);
        Task<ServiceResult<IEnumerable<MessageRecentIndexDto>>> GetRecentContactAsync(string sellerId, int pageNumber, int pageSize);
        Task<ServiceResult<IEnumerable<ProductBestSaleIndexDto>>> GetTop3BestSellingProductsAsync(string sellerId);
        Task<ServiceResult<double>> GetTotalOrderCountAsync(string sellerId);
        Task<ServiceResult<double>> GetTotalRevenueAsync(string sellerId);
        Task<ServiceResult<ReportIndexDto>> ReportOrderCountDay(string sellerId);
        Task<ServiceResult<ReportIndexDto>> ReportOrderCountMonth(string sellerId, int offsetMonth = 0);
        Task<ServiceResult<ReportIndexDto>> ReportOrderCountWeek(string sellerId, int offsetWeek = 0);
        Task<ServiceResult<ReportIndexDto>> ReportOrderCountYear(string sellerId, int offsetYear = 0);
        Task<ServiceResult<ReportIndexDto>> ReportQuantityProduct(string sellerId);
        Task<ServiceResult<ReportIndexDto>> ReportRatingAsync(string sellerId);
        Task<ServiceResult<ReportIndexDto>> ReportRevenueDay(string sellerId);
        Task<ServiceResult<ReportIndexDto>> ReportRevenueMonth(string sellerId, int offsetMonth = 0);
        Task<ServiceResult<ReportIndexDto>> ReportRevenueWeek(string sellerId, int offsetWeek = 0);
        Task<ServiceResult<ReportIndexDto>> ReportRevenueYear(string sellerId, int offsetYear = 0);
    }
}