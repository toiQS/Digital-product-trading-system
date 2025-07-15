using DPTS.Applications.Seller.conplaints.Dtos;
using DPTS.Applications.Seller.conplaints.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.conplaints.Handles
{
    public class GetComplaintsHandler : IRequestHandler<GetComplaintsQuery, ServiceResult<IEnumerable<ComplaintListItemDto>>>
    {
        private readonly IComplaintRepository _complaintRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly ILogger<GetComplaintsHandler> _logger;
        private readonly IStoreRepository _storeRepository;

        public GetComplaintsHandler(
            IComplaintRepository complaintRepository,
            IProductRepository productRepository,
            IUserProfileRepository userProfileRepository,
            ILogger<GetComplaintsHandler> logger,
            IStoreRepository storeRepository)
        {
            _complaintRepository = complaintRepository;
            _productRepository = productRepository;
            _userProfileRepository = userProfileRepository;
            _logger = logger;
            _storeRepository = storeRepository;
        }

        public async Task<ServiceResult<IEnumerable<ComplaintListItemDto>>> Handle(GetComplaintsQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching complaints for SellerId={SellerId}", query.SellerId);

            var result = new List<ComplaintListItemDto>();

            // 1. Kiểm tra hồ sơ người bán
            var profile = await _userProfileRepository.GetByUserIdAsync(query.SellerId);
            if (profile == null)
            {
                _logger.LogError("Seller profile not found. SellerId={SellerId}", query.SellerId);
                return ServiceResult<IEnumerable<ComplaintListItemDto>>.Error("Không tìm thấy hồ sơ người bán.");
            }

            // 2. Kiểm tra cửa hàng gắn với seller
            var store = await _storeRepository.GetByUserIdAsync(profile.UserId);
            if (store == null)
            {
                _logger.LogError("Store not found for SellerId={SellerId}", query.SellerId);
                return ServiceResult<IEnumerable<ComplaintListItemDto>>.Error("Không tìm thấy cửa hàng của người bán.");
            }

            // 3. Lấy danh sách sản phẩm thuộc cửa hàng
            var products = await _productRepository.GetByStoreAsync(store.StoreId);
            var complaints = new List<Complaint>();

            foreach (var product in products)
            {
                var productComplaints = await _complaintRepository.GetByProductIdAsync(product.ProductId);
                complaints.AddRange(productComplaints);
            }

            // 4. Mapping từng complaint thành DTO
            foreach (var complaint in complaints)
            {
                var buyerProfile = await _userProfileRepository.GetByUserIdAsync(complaint.UserId);
                if (buyerProfile == null)
                {
                    _logger.LogError("Buyer profile not found for ComplaintId={ComplaintId}, UserId={UserId}", complaint.ComplaintId, complaint.UserId);
                    return ServiceResult<IEnumerable<ComplaintListItemDto>>.Error("Không tìm thấy thông tin người khiếu nại.");
                }

                var dto = new ComplaintListItemDto
                {
                    ComplaintId = complaint.ComplaintId,
                    Title = complaint.Title,
                    ContentPreview = complaint.Description,
                    CreatedAt = complaint.CreatedAt,
                    OrderCode = complaint.EscrowId,
                    Status = EnumHandle.HandleComplaintStatus(complaint.Status),
                    CustomerName = buyerProfile.FullName ?? "Error",
                    ImageUrl = buyerProfile.ImageUrl ?? "Error"
                };

                result.Add(dto);
            }

            return ServiceResult<IEnumerable<ComplaintListItemDto>>.Success(result);
        }
    }
}
