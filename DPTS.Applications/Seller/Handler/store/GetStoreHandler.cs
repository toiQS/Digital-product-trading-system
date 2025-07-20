using DPTS.Applications.Seller.Dtos.store;
using DPTS.Applications.Seller.Query.store;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.Handler.store
{
    public class GetStoreHandler : IRequestHandler<GetStoreQuery,ServiceResult<DetailStoreDto>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly ILogger<GetStoreHandler> _logger;

        public GetStoreHandler(IUserRepository userRepository, IStoreRepository storeRepository, ILogger<GetStoreHandler> logger)
        {
            _userRepository = userRepository;
            _storeRepository = storeRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<DetailStoreDto>> Handle(GetStoreQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling get detail store by seller id");
            var seller = await _userRepository.GetByIdAsync(request.SellerId);
            if (seller == null)
            {
                _logger.LogError($"Not found seller by seller id:{request.SellerId}");
                return ServiceResult<DetailStoreDto>.Error("Không tìm thấy thông tin người bán");
            }
            var store = await _storeRepository.GetByUserIdAsync(request.SellerId);
            if (store == null)
            {
                _logger.LogError($"Not found store of seller by sellerId: {request.SellerId}");
                return ServiceResult<DetailStoreDto>.Error("Không tìm thấy cửa hàng");
            }
            var dto = new DetailStoreDto()
            {
                StoreId  = store.StoreId,
                StoreName = store.StoreName,
                StoreImage = store.StoreImage,
            };
            return ServiceResult<DetailStoreDto>.Success(dto);
        }
    }
}
