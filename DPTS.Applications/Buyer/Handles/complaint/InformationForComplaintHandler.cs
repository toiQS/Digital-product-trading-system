using DPTS.Applications.Buyer.Dtos.complaint;
using DPTS.Applications.Buyer.Queries.complaint;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles.complaint
{
    public class InformationForComplaintHandler : IRequestHandler<InformationForComplaintQuery, ServiceResult<InformationForComplaintDto>>
    {
        private readonly IEscrowRepository _escrowRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IComplaintRepository _complaintRepository;
        private readonly ILogger<InformationForComplaintHandler> _logger;

        public InformationForComplaintHandler(
            IEscrowRepository escrowRepository,
            IOrderItemRepository orderItemRepository,
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            IProductImageRepository productImageRepository,
            IUserProfileRepository userProfileRepository,
            IStoreRepository storeRepository,
            IComplaintRepository complaintRepository,
            ILogger<InformationForComplaintHandler> logger)
        {
            _escrowRepository = escrowRepository;
            _orderItemRepository = orderItemRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _productImageRepository = productImageRepository;
            _userProfileRepository = userProfileRepository;
            _storeRepository = storeRepository;
            _complaintRepository = complaintRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<InformationForComplaintDto>> Handle(InformationForComplaintQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling complaint info request for UserId = {UserId}", request.UserId);

            // Lấy thông tin người dùng
            var profile = await _userProfileRepository.GetByUserIdAsync(request.UserId);
            if (profile == null)
            {
                _logger.LogError("User profile not found for UserId = {UserId}", request.UserId);
                return ServiceResult<InformationForComplaintDto>.Error("Không tìm thấy thông tin người dùng.");
            }

            // Lấy danh sách đơn hàng của người dùng
            var orders = await _orderRepository.GetByBuyerAsync(profile.UserId, includeItems: true);
            if (!orders.Any())
            {
                _logger.LogInformation("No orders found for UserId = {UserId}", request.UserId);
                return ServiceResult<InformationForComplaintDto>.Error("Không có đơn hàng nào.");
            }

            var result = new InformationForComplaintDto();

            // Duyệt từng đơn hàng để lấy các sản phẩm đã mua
            foreach (var order in orders)
            {
                if (order.OrderItems == null || !order.OrderItems.Any())
                    continue;

                foreach (var item in order.OrderItems)
                {
                    // Lấy thông tin sản phẩm
                    var product = await _productRepository.GetByIdAsync(item.ProductId);
                    if (product == null)
                    {
                        _logger.LogWarning("Product not found: ProductId = {ProductId}", item.ProductId);
                        continue;
                    }

                    // Lấy ảnh đại diện chính của sản phẩm
                    var image = await _productImageRepository.GetPrimaryAsync(product.ProductId);
                    if (image == null)
                    {
                        _logger.LogWarning("Primary image not found for ProductId = {ProductId}", product.ProductId);
                        continue;
                    }

                    // Lấy thông tin cửa hàng
                    var store = await _storeRepository.GetByIdAsync(product.StoreId);
                    if (store == null)
                    {
                        _logger.LogWarning("Store not found: StoreId = {StoreId}", product.StoreId);
                        continue;
                    }

                    result.OrderItemPurchaseds.Add(new OrderItemPurchased
                    {
                        ProductName = product.ProductName,
                        BuyAt = order.UpdatedAt,
                        OrderItemId = item.OrderItemId
                    });
                }
            }

            // Lấy 10 khiếu nại gần nhất của người dùng
            var allComplaints = await _complaintRepository.GetAllAsync();
            var userComplaints = allComplaints
                .Where(x => x.UserId == request.UserId)
                .OrderByDescending(x => x.CreatedAt)
                .Take(10)
                .ToList();

            result.RecentlyComplaint = userComplaints.Select(x => new RecentlyComplaintDto
            {
                EscrowId = x.EscrowId,
                ComplaintAt = x.CreatedAt,
                Status = EnumHandle.HandleComplaintStatus(x.Status)
            }).ToList();

            _logger.LogInformation("Returned {PurchasedCount} purchased items and {ComplaintCount} complaints for UserId = {UserId}",
                result.OrderItemPurchaseds.Count, result.RecentlyComplaint.Count, request.UserId);

            return ServiceResult<InformationForComplaintDto>.Success(result);
        }
    }
}
