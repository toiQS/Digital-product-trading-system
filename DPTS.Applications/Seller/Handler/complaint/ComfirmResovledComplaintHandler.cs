using DPTS.Applications.Seller.Query.complaint;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.Handler.complaint
{
    public class ComfirmResovledComplaintHandler : IRequestHandler<ComfirmResovledComplaintCommand, ServiceResult<string>>
    {
        private readonly IComplaintRepository _complaintRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogRepository _logRepository;
        private readonly ILogger<ComfirmResovledComplaintHandler> _logger;

        public ComfirmResovledComplaintHandler(IComplaintRepository complaintRepository,
                                               IUserProfileRepository userProfileRepository,
                                               IStoreRepository storeRepository,
                                               IProductRepository productRepository,
                                               ILogRepository logRepository,
                                               ILogger<ComfirmResovledComplaintHandler> logger)
        {
            _complaintRepository = complaintRepository;
            _userProfileRepository = userProfileRepository;
            _storeRepository = storeRepository;
            _productRepository = productRepository;
            _logRepository = logRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<string>> Handle(ComfirmResovledComplaintCommand request, CancellationToken cancellationToken)
        {
            var sellerProfile = await _userProfileRepository.GetByUserIdAsync(request.SellerId);
            if (sellerProfile == null)
            {
                _logger.LogWarning("Seller profile not found for user ID: {UserId}", request.SellerId);
                return ServiceResult<string>.Error("Không tìm thấy người bán.");
            }
            var store = await _storeRepository.GetByUserIdAsync(request.SellerId);
            if (store == null)
            {
                _logger.LogWarning("Store not found for user ID: {UserId}", request.SellerId);
                return ServiceResult<string>.Error("Không tìm thấy cửa hàng của người bán.");
            }
            var complaint = await _complaintRepository.GetByIdAsync(request.ComplaintId);
            if(complaint == null)
            {
                _logger.LogWarning("Complaint not found for ID: {ComplaintId}", request.ComplaintId);
                return ServiceResult<string>.Error("Không tìm thấy khiếu nại.");
            }
            var product = await _productRepository.GetByIdAsync(complaint.ProductId);
            if (product == null)
            {
                _logger.LogWarning("Product not found for ID: {ProductId}", complaint.ProductId);
                return ServiceResult<string>.Error("Không tìm thấy sản phẩm liên quan đến khiếu nại.");
            }
            if(product.StoreId != store.StoreId)
            {
                _logger.LogWarning("Product {ProductId} does not belong to store {StoreId}", product.ProductId, store.StoreId);
                return ServiceResult<string>.Error("Sản phẩm không thuộc về cửa hàng của người bán.");
            }  
            if (complaint.Status != Domains.ComplaintStatus.Pending)
            {
                _logger.LogWarning("Complaint {ComplaintId} is not in pending status", request.ComplaintId);
                return ServiceResult<string>.Error("Khiếu nại không ở trạng thái chờ xử lý.");
            }
            complaint.Status = Domains.ComplaintStatus.Resolved;
            var log = new Log()
            {
                LogId = Guid.NewGuid().ToString(),
                Action = "ConfirmResolvedComplaint",
                CreatedAt = DateTime.UtcNow,
                TargetId = complaint.ComplaintId,
                TargetType = "Complaint",
                UserId = request.SellerId,
            };
            try
            {
                await _complaintRepository.UpdateAsync(complaint);
                await _logRepository.AddAsync(log);
                return ServiceResult<string>.Success("Khiếu nại đã được xác nhận là đã giải quyết thành công.");
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error confirming resolved complaint for ID: {ComplaintId}", request.ComplaintId);
                return ServiceResult<string>.Error("Đã xảy ra lỗi khi xác nhận khiếu nại đã giải quyết.");
            }
        }
    }
}
