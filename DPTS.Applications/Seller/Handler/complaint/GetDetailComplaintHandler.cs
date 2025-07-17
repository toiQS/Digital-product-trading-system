using DPTS.Applications.Seller.Dtos.complaint;
using DPTS.Applications.Seller.Query.complaint;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.Handler.complaint
{
    public class GetDetailComplaintHandler : IRequestHandler<GetDetailComplaintQuery, ServiceResult<GetDetailComplaintDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IComplaintRepository _complaintRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IProductRepository _productRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly ILogger<GetDetailComplaintHandler> _logger;

        public GetDetailComplaintHandler(IUserRepository userRepository,
                                         IComplaintRepository complaintRepository,
                                         IStoreRepository storeRepository,
                                         IProductRepository productRepository,
                                         IUserProfileRepository userProfileRepository,
                                         ILogger<GetDetailComplaintHandler> logger)
        {
            _userRepository = userRepository;
            _complaintRepository = complaintRepository;
            _storeRepository = storeRepository;
            _productRepository = productRepository;
            _userProfileRepository = userProfileRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<GetDetailComplaintDto>> Handle(GetDetailComplaintQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetDetailComplaintQuery for ComplaintId: {ComplaintId} and SellerId: {SellerId}", request.ComplaintId, request.SellerId);
            var seller = await _userRepository.GetByIdAsync(request.SellerId);
            if (seller == null)
            {
                _logger.LogWarning("Seller with ID {SellerId} not found", request.SellerId);
                return ServiceResult<GetDetailComplaintDto>.Error("Người bán không tồn tại.");
            }
            var store = await _storeRepository.GetByUserIdAsync(request.SellerId);
            if (store == null)
            {
                _logger.LogWarning("Store for SellerId {SellerId} not found", request.SellerId);
                return ServiceResult<GetDetailComplaintDto>.Error("Không tìm thấy gian hàng");
            }
            var complaint = await _complaintRepository.GetByIdAsync(request.ComplaintId);
            if (complaint == null)
            {
                _logger.LogWarning("Complaint with ID {ComplaintId} not found", request.ComplaintId);
                return ServiceResult<GetDetailComplaintDto>.Error("Khiếu nại không tồn tại.");
            }
            var product = await _productRepository.GetByIdAsync(complaint.ProductId);
            if (product == null)
            {
                _logger.LogWarning("Product for ComplaintId {ComplaintId} not found", request.ComplaintId);
                return ServiceResult<GetDetailComplaintDto>.Error("Sản phẩm không tồn tại.");
            }
            if(product.StoreId != store.StoreId)
            {
                _logger.LogWarning("Product with ID {ProductId} does not belong to StoreId {StoreId}", product.ProductId, store.StoreId);
                return ServiceResult<GetDetailComplaintDto>.Error("Sản phẩm không thuộc gian hàng của bạn.");
            }
            var buyerProfile = await _userProfileRepository.GetByUserIdAsync(complaint.UserId);
            if (buyerProfile == null)
            {
                _logger.LogWarning("Buyer with ID {UserId} not found for ComplaintId {ComplaintId}", complaint.UserId, request.ComplaintId);
                return ServiceResult<GetDetailComplaintDto>.Error("Người mua không tồn tại.");
            }
            var buyer = await _userRepository.GetByIdAsync(complaint.UserId);
            if (buyer == null)
            {
                _logger.LogWarning("User with ID {UserId} not found for ComplaintId {ComplaintId}", complaint.UserId, request.ComplaintId);
                return ServiceResult<GetDetailComplaintDto>.Error("Người mua không tồn tại.");
            }
            var complaintDto = new GetDetailComplaintDto
            {
                ComplaintId = complaint.ComplaintId,
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = complaint.Description,
                Status = EnumHandle.HandleComplaintStatus(complaint.Status),
                CreatedAt = complaint.CreatedAt,
                BuyerId = complaint.UserId,
                BuyerName = buyerProfile.FullName,
                EscrowId = complaint.EscrowId,
                MethodContact = buyer.Email ?? buyerProfile.Phone,
                Images = complaint.Images.Select(img => img.ImagePath).ToList(),
                Title = complaint.Title
            };
            return ServiceResult<GetDetailComplaintDto>.Success(complaintDto);
        }
    }
}
