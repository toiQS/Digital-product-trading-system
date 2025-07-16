using DPTS.Applications.Seller.Dtos.complaint;
using DPTS.Applications.Seller.Query.complaint;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.Handler.complaint
{
    public class GetComplaintOverViewHandler : IRequestHandler<GetComplaintOverviewQuery, ServiceResult<IEnumerable<ComplaintOverviewDto>>>
    {
        private readonly ILogger<GetComplaintOverViewHandler> _logger;
        private readonly IProductRepository _productRepository;
        private readonly IComplaintRepository _complaintRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IStoreRepository _storeRepository;

        public GetComplaintOverViewHandler(
            ILogger<GetComplaintOverViewHandler> logger,
            IProductRepository productRepository,
            IComplaintRepository complaintRepository,
            IUserProfileRepository userProfileRepository,
            IStoreRepository storeRepository)
        {
            _logger = logger;
            _productRepository = productRepository;
            _complaintRepository = complaintRepository;
            _userProfileRepository = userProfileRepository;
            _storeRepository = storeRepository;
        }

        public async Task<ServiceResult<IEnumerable<ComplaintOverviewDto>>> Handle(GetComplaintOverviewQuery query, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Getting complaint overview for seller: {SellerId}", query.SellerId);

            // 1. Lấy thông tin người bán
            var profileSeller = await _userProfileRepository.GetByUserIdAsync(query.SellerId);
            if (profileSeller == null)
            {
                _logger.LogError("Seller profile not found. SellerId={SellerId}", query.SellerId);
                return ServiceResult<IEnumerable<ComplaintOverviewDto>>.Error("Không tìm thấy hồ sơ người bán.");
            }

            // 2. Lấy cửa hàng tương ứng
            var store = await _storeRepository.GetByUserIdAsync(profileSeller.UserId);
            if (store == null)
            {
                _logger.LogError("Store not found for seller. UserId={UserId}", profileSeller.UserId);
                return ServiceResult<IEnumerable<ComplaintOverviewDto>>.Error("Không tìm thấy cửa hàng của người bán.");
            }

            // 3. Lấy tất cả sản phẩm của store
            var products = await _productRepository.GetByStoreAsync(store.StoreId);

            // 4. Lấy tất cả khiếu nại của các sản phẩm
            var complaints = new List<Complaint>();
            foreach (var product in products)
            {
                var productComplaints = await _complaintRepository.GetByProductIdAsync(product.ProductId);
                complaints.AddRange(productComplaints);
            }

            // 5. Tính toán thống kê
            var total = complaints.Count;
            var today = DateTime.Today;
            var newComplaints = complaints.Where(x => x.CreatedAt >= today && x.CreatedAt < today.AddDays(1)).ToList();
            var pendingComplaints = complaints.Where(x => x.Status == ComplaintStatus.Pending).ToList();
            var resolvedComplaints = complaints.Where(x => x.Status == ComplaintStatus.Resolved).ToList();

            // 6. Tính tỷ lệ giải quyết nếu có complaint
            var resolveRate = total > 0
                ? Math.Round((double)resolvedComplaints.Count * 100 / total, 2)
                : 0;

            // 7. Trả về kết quả
            var overview = new List<ComplaintOverviewDto>
            {
                new ComplaintOverviewDto
                {
                    Name = "Tổng khiếu nại",
                    Information = $"{newComplaints.Count} khiếu nại mới hôm nay",
                    Value = total
                },
                new ComplaintOverviewDto
                {
                    Name = "Chờ xử lý",
                    Information = "Cần xử lý",
                    Value = pendingComplaints.Count
                },
                new ComplaintOverviewDto
                {
                    Name = "Đã giải quyết",
                    Information = $"Tỷ lệ giải quyết {resolveRate}%",
                    Value = resolvedComplaints.Count
                }
            };

            return ServiceResult<IEnumerable<ComplaintOverviewDto>>.Success(overview);
        }
    }
}
