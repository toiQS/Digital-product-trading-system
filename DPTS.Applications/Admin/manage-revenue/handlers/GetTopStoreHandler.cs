using DPTS.Applications.Admin.manage_revenue.dtos;
using DPTS.Applications.Admin.manage_revenue.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using LinqKit;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Admin.manage_revenue.handlers
{
    public class GetTopStoreHandler : IRequestHandler<GetTopStoreQuery, ServiceResult<TopStoreDto>>
    {
        private readonly ILogger<GetTopStoreHandler> _logger;
        private readonly IUserRepository _userRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IEscrowRepository _escrowRepository;
        private readonly IProductReviewRepository _productReviewRepository;
        private readonly IProductRepository _productRepository;

        public GetTopStoreHandler(ILogger<GetTopStoreHandler> logger,
                                  IUserRepository userRepository,
                                  IStoreRepository storeRepository,
                                  IEscrowRepository escrowRepository,
                                  IProductReviewRepository productReviewRepository,
                                  IProductRepository productRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _storeRepository = storeRepository;
            _escrowRepository = escrowRepository;
            _productReviewRepository = productReviewRepository;
            _productRepository = productRepository;
        }

        public async Task<ServiceResult<TopStoreDto>> Handle(GetTopStoreQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling get top store");
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("Not found user with Id: {UserId}",request.UserId);
                return ServiceResult<TopStoreDto>.Error("không tìm thấy người dùng");
            }
            if (user.RoleId != "Admin")
            {
                _logger.LogError("User current don't have enough access books");
                return ServiceResult<TopStoreDto>.Error("Người dùng hiện tại không đủ quyền truy cập");
            }
            var stores = await _storeRepository.GetAllAsync();
            var escrows = (await _escrowRepository.GetAllAsync()).Where(x => x.Status == Domains.EscrowStatus.Done);
            var groupedByStoreId = from escrow in escrows
                                   group escrow by escrow.StoreId
                                   into g
                                   select new GroupedResult
                                   {
                                       StoreId = g.Key,
                                       Revenue = g.Sum(x => x.ActualAmount)
                                   };
            var result = new TopStoreDto();
            var sorted= groupedByStoreId.OrderByDescending(x => x.Revenue).Take(10).ToList();
            sorted.ForEach(async x =>
            {
                var store = await _storeRepository.GetByIdAsync(x.StoreId);
                if (store == null)
                {
                    _logger.LogError("Not found store with Id: {StoreId}", x.StoreId);
                }
                var products = await _productRepository.GetByStoreAsync(x.StoreId);
                double totalRatingStore = 0;
                products.ForEach(async p =>
                {
                    var revews = await _productReviewRepository.GetByProductIdAsync(p.ProductId);
                    totalRatingStore += revews.Average(x => x.RatingOverall);
                });
                var index = new TopStoreIndexDto()
                {
                    StoreName = store.StoreName ??"Error",
                    AverageRating = totalRatingStore/products.Count(x => x.Status == Domains.ProductStatus.Available),
                    CountProduct = products.Count(),
                    Revenue = ConvertUnit(x.Revenue),
                    StoreId = store.StoreId,
                    StoreImage = store.StoreImage,
                };
                result.Indexs.Add(index);
            });
            return ServiceResult<TopStoreDto>.Success(result);
        }
        private class GroupedResult
        {
            public string StoreId { get; set; }
            public decimal Revenue { get; set; }
        }
        private string ConvertUnit(decimal value)
        {
            if (value < 1000000)
                return value + " vnđ";
            if (value >= 1000000 && value < 9999999)
                return value / 1000000 + " triệu vnđ";

            if (value >= 1000000000 && value < 9999999999)
                return value / 1000000 + " tỉ vnđ";
            else return "Vượt hạn mức";
        }
    }
}
