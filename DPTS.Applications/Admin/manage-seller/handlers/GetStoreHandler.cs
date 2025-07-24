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
            {
                _logger.LogError("Not found with Id: {d}", request.UserId);
                return ServiceResult<StoreDto>.Error("Không tìm thấy người dùng");
            }
            if (user.RoleId != "Admin")
            {
                _logger.LogError("User current don't have enough access books");
                return ServiceResult<StoreDto>.Error("Người dùng hiện tại không đủ quyền truy cập");
            }
            var result = new StoreDto();
            var stores = await _storeRepository.GetAllAsync();
            if (string.IsNullOrEmpty(request.Condition.Text))
            {
                stores = stores.Where(x => x.StoreName.Contains(request.Condition.Text) || x.StoreId.Contains(request.Condition.Text));
            }
            if (request.Condition.StoreStatus != 0)
                stores = stores.Where(x => x.Status == request.Condition.StoreStatus);
            stores.ForEach(async s =>
            {

                var products = await _productRepository.GetByStoreAsync(s.StoreId);

                int countProudctAvalible = products.Count(x => x.Status == Domains.ProductStatus.Available);
                double totalrating = 0;
                products.ForEach(async p =>
                {
                    totalrating += await _productReviewRepository.GetAverageOverallRatingAsync(p.ProductId);
                });


                var escrows = (await _escrowRepository.GetAllAsync()).Where(x => x.StoreId == s.StoreId && x.Status == Domains.EscrowStatus.Done);
                var revenue = escrows.Sum(x => x.ActualAmount);
                var index = new StoreIndexDto()
                {
                    StoreName = s.StoreName,
                    CountProduct = products.Count(),
                    StoreId = s.StoreId,
                    Rating = totalrating / countProudctAvalible,
                    Revenue = revenue,
                    Status = EnumHandle.HandleStoreStatus(s.Status),
                    StoreImage = s.StoreImage,
                };
                result.StoreIndices.Add(index);
            });
            result.StoreIndices.OrderByDescending(x => x.Revenue)
                .OrderByDescending(x => x.Rating)
                .Skip((request.PageCount - 1)* request.PageSize)
                .Take(request.PageSize);
            return ServiceResult<StoreDto>.Success(result);
        }
    }
}
