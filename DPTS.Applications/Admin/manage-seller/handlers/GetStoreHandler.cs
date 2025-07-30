using DPTS.Applications.Admin.manage_seller.dtos;
using DPTS.Applications.Admin.manage_seller.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using LinqKit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Admin.manage_seller.handlers
{
    public class GetStoreHandler : IRequestHandler<GetStoreListQuery, ServiceResult<StoreDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly IEscrowRepository _escrowRepository;
        private readonly ILogger<GetStoreHandler> _logger;

        public GetStoreHandler(IUserRepository userRepository, IStoreRepository storeRepository, IProductRepository productRepository, IProductReviewRepository productReviewRepository, IEscrowRepository escrowRepository, ILogger<GetStoreHandler> logger)
        {
            _userRepository = userRepository;
            _storeRepository = storeRepository;
            _productRepository = productRepository;
            _productReviewRepository = productReviewRepository;
            _escrowRepository = escrowRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<StoreDto>> Handle(GetStoreListQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling get stores with role admin");

            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
                return ServiceResult<StoreDto>.Error("Không tìm thấy người dùng");

            if (user.RoleId != "Admin")
                return ServiceResult<StoreDto>.Error("Người dùng hiện tại không đủ quyền truy cập");

            // 1. Filter store list
            var stores = await _storeRepository.GetAllAsync();
            

            if (!string.IsNullOrWhiteSpace(request.Condition.Text))
            {
                stores = stores.Where(s =>
                    s.StoreName.ToLower().Contains(request.Condition.Text.ToLower()) || s.StoreId.Contains(request.Condition.Text.ToLower()));
            }
            var checkpoint = stores;
            if (request.Condition.StoreStatus != 0)
            {
                stores = stores.Where(s => s.Status == request.Condition.StoreStatus);
            }
            var checkpoint2 = stores;

            // 2. Load all data needed
            var allProducts = await _productRepository.SearchAsync(); // better to optimize this
            var allReviews = await _productReviewRepository.GetAllAsync(); // need to add this method
            var allEscrows = await _escrowRepository.GetAllAsync();

            // 3. Build result
            var result = new StoreDto();
            foreach (var s in stores)
            {
                var products = allProducts.Where(p => p.StoreId == s.StoreId).ToList();
                var productIds = products.Select(p => p.ProductId).ToList();

                var countAvailable = products.Count(p => p.Status == Domains.ProductStatus.Available);

                double totalRating = allReviews
                    .Where(r => productIds.Contains(r.ProductId))
                    .Average(r => (double?)r.RatingOverall) ?? 0;

                var revenue = allEscrows
                    .Where(e => e.StoreId == s.StoreId && e.Status == Domains.EscrowStatus.Done)
                    .Sum(e => e.ActualAmount);

                result.StoreIndices.Add(new StoreIndexDto
                {
                    StoreName = s.StoreName,
                    CountProduct = products.Count,
                    StoreId = s.StoreId,
                    Rating = countAvailable > 0 ? totalRating : 0,
                    Revenue = revenue,
                    Status = EnumHandle.HandleStoreStatus(s.Status),
                    StoreImage = s.StoreImage
                });
            }
            var checkPoint3 = result.StoreIndices;
            // 4. Sorting + paging
            var pagedStores = result.StoreIndices
                .OrderByDescending(x => x.Revenue)
                .ThenByDescending(x => x.Rating)
                .Skip((request.PageCount - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToList();

            return ServiceResult<StoreDto>.Success(result);
        }

    }
}
