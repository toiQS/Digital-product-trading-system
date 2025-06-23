using DPTS.Applications.Sellers.conplaints.Dtos;
using DPTS.Applications.Sellers.conplaints.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

public class GetComplaintsHandler : IRequestHandler<GetComplaintsQuery, ServiceResult<IEnumerable<ComplaintListItemDto>>>
{
    private readonly IComplaintRepository _complaintRepository;
    private readonly ILogger<GetComplaintsHandler> _logger;
    private readonly IStoreRepository _storeRepository;

    public GetComplaintsHandler(
        IComplaintRepository complaintRepository,
        ILogger<GetComplaintsHandler> logger,
        IStoreRepository storeRepository)
    {
        _complaintRepository = complaintRepository;
        _logger = logger;
        _storeRepository = storeRepository;
    }

    public async Task<ServiceResult<IEnumerable<ComplaintListItemDto>>> Handle(GetComplaintsQuery query, CancellationToken cancellationToken)
    {
        try
        {
            _logger.LogInformation("Lấy danh sách khiếu nại của seller {SellerId}", query.SellerId);
            var store = await _storeRepository.GetByUserIdAsync(query.SellerId);
            if (store == null)
                return ServiceResult<IEnumerable<ComplaintListItemDto>>.Error("Không tìm thấy cửa hàng.");

            var complaints = (await _complaintRepository.GetsAsync(includeProduct: true, includeUser: true))
                .Where(x => x.Product != null && x.Product.StoreId == store.StoreId)
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
            }).ToList();

            return ServiceResult<IEnumerable<ComplaintListItemDto>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Lỗi khi lấy danh sách khiếu nại của seller {SellerId}", query.SellerId);
            return ServiceResult<IEnumerable<ComplaintListItemDto>>.Error("Không thể lấy danh sách khiếu nại.");
        }
    }
}
