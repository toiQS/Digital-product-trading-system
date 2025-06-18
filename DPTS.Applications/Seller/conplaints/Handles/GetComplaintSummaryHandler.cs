using DPTS.Applications.Seller.conplaints.Dtos;
using DPTS.Applications.Seller.conplaints.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.conplaints.Handles
{
    public class GetComplaintSummaryHandler : IRequestHandler<GetComplaintSummaryQuery,ServiceResult<IEnumerable<ComplaintSummaryDto>>>
    {
        private readonly ILogger<GetComplaintSummaryHandler> _logger;
        private readonly IProductRepository _productRepository;
        private readonly IComplaintRepository _complaintRepository;

        public GetComplaintSummaryHandler(
            ILogger<GetComplaintSummaryHandler> logger,
            IProductRepository productRepository,
            IComplaintRepository complaintRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            _complaintRepository = complaintRepository;
        }

        public async Task<ServiceResult<IEnumerable<ComplaintSummaryDto>>> Handle(GetComplaintSummaryQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Bắt đầu xử lý thống kê khiếu nại cho seller {SellerId}", query.SellerId);

                var complaints = (await _complaintRepository.GetsAsync(includeProduct: true))
                    .Where(x => x.Product.SellerId == query.SellerId)
                    .ToList();

                var totalCount = complaints.Count;

                var today = DateTime.Today;
                var newComplaintCount = complaints.Count(x =>
                    x.CreatedAt >= today && x.CreatedAt < today.AddDays(1));

                var pendingCount = complaints.Count(x => x.Status == Domains.ComplaintStatus.Pending);
                var resolvedCount = complaints.Count(x => x.Status == Domains.ComplaintStatus.Resolved);

                var result = new List<ComplaintSummaryDto>
                {
                    new ComplaintSummaryDto
                    {
                        Name = "Tổng khiếu nại",
                        Value = totalCount,
                        Information = $"{newComplaintCount} khiếu nại mới hôm nay"
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

                return ServiceResult<IEnumerable<ComplaintSummaryDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy thống kê khiếu nại cho seller {SellerId}", query.SellerId);
                return ServiceResult<IEnumerable<ComplaintSummaryDto>>.Error("Không thể lấy thống kê khiếu nại.");
            }
        }
    }
}
