using DPTS.Applications.Seller.conplaints.Dtos;
using DPTS.Applications.Seller.conplaints.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.conplaints.Handles
{
    public class GetComplaintsHandler
    {
        private readonly IComplaintRepository _complaintRepository;
        private readonly ILogger<GetComplaintsHandler> _logger;

        public GetComplaintsHandler(
            IComplaintRepository complaintRepository,
            ILogger<GetComplaintsHandler> logger)
        {
            _complaintRepository = complaintRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<ComplaintListItemDto>>> Handle(GetComplaintsQuery query, CancellationToken cancellationToken = default)
        {
            try
            {
                _logger.LogInformation("Lấy danh sách khiếu nại của seller {SellerId}", query.SellerId);

                var complaints = (await _complaintRepository.GetsAsync(includeProduct: true, includeUser: true))
                    .Where(x => x.Product.SellerId == query.SellerId)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToList();

                var result = complaints.Select(x => new ComplaintListItemDto
                {
                    ComplaintId = x.ComplaintId,
                    CustomerName = x.User?.FullName ?? "Không rõ",
                    ContentPreview = x.Description,
                    CreatedAt = x.CreatedAt,
                    ImageUrl = x.User?.ImageUrl ?? "/images/default-user.png",
                    OrderCode = x.OrderId,
                    Status = EnumHandle.HandleComplaintStatus(x.Status),
                    Title = x.Title,
                }).ToList().OrderByDescending(x => x.CreatedAt);

                return ServiceResult<IEnumerable<ComplaintListItemDto>>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi khi lấy danh sách khiếu nại của seller {SellerId}", query.SellerId);
                return ServiceResult<IEnumerable<ComplaintListItemDto>>.Error("Không thể lấy danh sách khiếu nại.");
            }
        }
    }
}
