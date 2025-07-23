using DPTS.Applications.Admin.manage_seller.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Admin.manage_seller.handlers
{
    public class UpdateStatusStoreHandler : IRequestHandler<UpdateStatusStoreCommand, ServiceResult<string>>
    {
        private readonly ILogger<UpdateStatusStoreHandler> _logger; 
        private readonly IUserRepository _userRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly ILogRepository _logRepository;

        public UpdateStatusStoreHandler(ILogger<UpdateStatusStoreHandler> logger, IUserRepository userRepository, IStoreRepository storeRepository, ILogRepository logRepository)
        {
            _logger = logger;
            _userRepository = userRepository;
            _storeRepository = storeRepository;
            _logRepository = logRepository;
        }

        public async Task<ServiceResult<string>> Handle(UpdateStatusStoreCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling update store");
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("Not found with Id: {d}", request.UserId);
                return ServiceResult<string>.Error("Không tìm thấy người dùng");
            }
            if (user.RoleId != "Admin")
            {
                _logger.LogError("User current don't have enough access books");
                return ServiceResult<string>.Error("Người dùng hiện tại không đủ quyền truy cập");
            }
            var store = await _storeRepository.GetByIdAsync(request.UserId);
            if (store == null)
            {
                _logger.LogError("Not found store with Id:{id}}",request.StoreId);
                return ServiceResult<string>.Error("Không tìm thấy cửa hàng");
            }
            var log = new Log()
            {
                LogId = Guid.NewGuid().ToString(),
                CreatedAt = DateTime.UtcNow,
                Description = $"{store.StoreId} vừa được cập nhật bởi admin {request.UserId}",
                Action = "Cập nhật tình trạng cửa hàng",
                TargetId = request.StoreId,
                TargetType ="Store",
                UserId = request.UserId,
            };
            try
            {
                store.Status = request.StoreStatus;
                await _storeRepository.UpdateAsync(store);
                await _logRepository.AddAsync(log);
                return ServiceResult<string>.Success("Cập nhật thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when update store");
                return ServiceResult<string>.Error("Lỗi khi cập nhật cửa hàng");
            }
        }
    }
}
