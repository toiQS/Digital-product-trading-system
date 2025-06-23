using DPTS.Applications.Sellers.conplaints.Dtos;
using DPTS.Applications.Sellers.conplaints.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Sellers.conplaints.Handles
{
    public class GetComplaintSummaryHandler : IRequestHandler<GetComplaintSummaryQuery, ServiceResult<IEnumerable<ComplaintSummaryDto>>>
    {
        private readonly ILogger<GetComplaintSummaryHandler> _logger;
        private readonly IProductRepository _productRepository;
        private readonly IComplaintRepository _complaintRepository;
        private readonly IStoreRepository _storeRepository;
        public GetComplaintSummaryHandler(
            ILogger<GetComplaintSummaryHandler> logger,
            IProductRepository productRepository,
            IComplaintRepository complaintRepository,
            IStoreRepository storeRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            _storeRepository = storeRepository;
            _complaintRepository = complaintRepository;
        }

        public async Task<ServiceResult<IEnumerable<ComplaintSummaryDto>>> Handle(GetComplaintSummaryQuery query, CancellationToken cancellationToken = default)
        {
            _logger.LogInformation("Bắt đầu xử lý thống kê khiếu nại cho SellerId: {SellerId}", query.SellerId);

            try
            {
                Domains.Store? store = await _storeRepository.GetByUserIdAsync(query.SellerId);
                if (store == null)
                {
                    _logger.LogWarning("Không tìm thấy store cho SellerId: {SellerId}", query.SellerId);
                    return ServiceResult<IEnumerable<ComplaintSummaryDto>>.Error("Không tìm thấy cửa hàng.");
                }

                List<Domains.Complaint> complaints = (await _complaintRepository.GetsAsync(includeProduct: true))
                    .Where(c => c.Product != null && c.Product.StoreId == store.StoreId)
                    .ToList();

                int totalCount = complaints.Count;
                DateTime today = DateTime.Today;
                int newTodayCount = complaints.Count(c => c.CreatedAt >= today && c.CreatedAt < today.AddDays(1));
                int pendingCount = complaints.Count(c => c.Status == Domains.ComplaintStatus.Pending);
                int resolvedCount = complaints.Count(c => c.Status == Domains.ComplaintStatus.Resolved);

                List<ComplaintSummaryDto> summaries = new()
                {
            new ComplaintSummaryDto
            {
                Name = "Tổng khiếu nại",
                Value = totalCount,
                Information = $"{newTodayCount} khiếu nại mới hôm nay"
            },
            new ComplaintSummaryDto
            {
                Name = "Chờ xử lý",
                Value = pendingCount,
                Information = pendingCount > 0 ? "Cần xử lý ngay." : "Không có khiếu nại đang chờ."
            },
            new ComplaintSummaryDto
            {
                Name = "Đã giải quyết",
                Value = resolvedCount,
                Information = $"Đã xử lý {resolvedCount} khiếu nại"
            }
        };

                return ServiceResult<IEnumerable<ComplaintSummaryDto>>.Success(summaries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi trong khi xử lý thống kê khiếu nại cho SellerId: {SellerId}", query.SellerId);
                return ServiceResult<IEnumerable<ComplaintSummaryDto>>.Error("Không thể lấy thống kê khiếu nại.");
            }
        }

    }
}
