using DPTS.Applications.Seller.Dtos.store;
using DPTS.Applications.Seller.Query.store;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using MediatR.Wrappers;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.Handler.store
{
    public class UpdateStoreHandler : IRequestHandler<UpdateStoreCommand, ServiceResult<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly ILogRepository _logRepository;
        private readonly ILogger<UpdateStoreHandler> _logger;

        public UpdateStoreHandler(IUserRepository userRepository, IStoreRepository storeRepository, ILogRepository logRepository, ILogger<UpdateStoreHandler> logger)
        {
            _userRepository = userRepository;
            _storeRepository = storeRepository;
            _logRepository = logRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<string>> Handle(UpdateStoreCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling update store");
            var seller = await _userRepository.GetByIdAsync(request.UserId);
            if (seller == null)
            {
                _logger.LogError($"Not found seller by seller id:{request.UserId}");
                return ServiceResult<string>.Error("Không tìm thấy thông tin người bán");
            }
            var store = await _storeRepository.GetByUserIdAsync(request.UserId);
            if (store == null)
            {
                _logger.LogError($"Not found store of seller by sellerId: {request.UserId}");
                return ServiceResult<string>.Error("Không tìm thấy cửa hàng");
            }
            var log = new Log()
            {
                LogId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                TargetId = request.StoreId,
                Action = "UPDATE-STORE",
                TargetType = "Store",
                UserId = request.UserId,
            };
            try
            {
                store.StoreName = request.UpdateStore.Name ?? store.StoreName;
                store.StoreImage = request.UpdateStore.Image ?? store.StoreImage;
                await _storeRepository.UpdateAsync(store);
                await _logRepository.AddAsync(log);
                return ServiceResult<string>.Success("Cập nhật thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error when update store: {store.StoreId}");
                return ServiceResult<string>.Error("Cập nhật không thành công");
            }
        }
    }
}
