using DPTS.Applications.Seller.Query.store;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.Handler.store
{
    public class AddAdjustmentRuleHandler : IRequestHandler<AddAdjustmentRuleCommand, ServiceResult<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IAdjustmentRuleRepository _adjustmentRuleRepository;
        private readonly ILogger<AddAdjustmentRuleHandler> _logger;
        private  readonly ILogRepository _logRepository;

        public AddAdjustmentRuleHandler(IUserRepository userRepository, IStoreRepository storeRepository, IAdjustmentRuleRepository adjustmentRuleRepository, ILogger<AddAdjustmentRuleHandler> logger, ILogRepository logRepository)
        {
            _userRepository = userRepository;
            _storeRepository = storeRepository;
            _adjustmentRuleRepository = adjustmentRuleRepository;
            _logger = logger;
            _logRepository = logRepository;
        }

        public async Task<ServiceResult<string>> Handle(AddAdjustmentRuleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling AddAdjustmentRuleCommand for UserId: {UserId}, StoreId: {StoreId}", request.UserId, request.StoreId);
            var user = await _userRepository.GetByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("User not found with UserId: {UserId}", request.UserId);
                return ServiceResult<string>.Error("Không tìm thấy người dùng");
            }

            var store = await _storeRepository.GetByIdAsync(request.StoreId);
            if (store == null)
            {
                _logger.LogError("Store not found with StoreId: {StoreId}", request.StoreId);
                return ServiceResult<string>.Error("Không tìm thấy cửa hàng");
            }

            if (store.UserId != request.UserId)
            {
                _logger.LogError("StoreId: {StoreId} không thuộc về UserId: {UserId}", store.StoreId, request.UserId);
                return ServiceResult<string>.Error("Cửa hàng không thuộc về người dùng này");
            }

            var adjustmentRule = new Domains.AdjustmentRule
            {
                RuleId = Guid.NewGuid().ToString(),
                Name = request.AdjustmentRule.Name,
                Description = request.AdjustmentRule.Description,
                Type = Domains.AdjustmentType.Discount,
                Scope = Domains.AdjustmentScope.PerProduct,
                MinOrderAmount = request.AdjustmentRule.MinOrderAmount,
                SourceId = store.StoreId,
                Source = Domains.AdjustmentSource.Seller,
                VoucherCode = request.AdjustmentRule.VoucherCode,
                IsPercentage = request.AdjustmentRule.IsPercentage,
                Value = request.AdjustmentRule.Value,
                MaxCap = request.AdjustmentRule.MaxCap,
                UsageLimit = request.AdjustmentRule.UsageLimit,
                PerUserLimit = request.AdjustmentRule.PerUserLimit,
                
                From = request.AdjustmentRule.From,
                To = request.AdjustmentRule.To
            };
            adjustmentRule.TargetLogic = request.AdjustmentRule.TargetLogic switch
            {
                TargetLogicForStore.Auto => Domains.TargetLogic.Auto,
                TargetLogicForStore.Voucher => Domains.TargetLogic.Voucher,
                _ => throw new ArgumentOutOfRangeException(nameof(request.AdjustmentRule.TargetLogic), "Invalid target logic")
            };
            adjustmentRule.Status = request.AdjustmentRule.Status switch
            {
                RuleStatusForStore.Active => Domains.RuleStatus.Active,
                RuleStatusForStore.Inactive => Domains.RuleStatus.Inactive,
                RuleStatusForStore.Expired => Domains.RuleStatus.Expired,
                _ => throw new ArgumentOutOfRangeException(nameof(request.AdjustmentRule.Status), "Invalid rule status")
            };
            var log = new Log
            {
                LogId = Guid.NewGuid().ToString(),
                UserId = request.UserId,
                Action = "AddAdjustmentRule",
                Description = $"Thêm quy tắc điều chỉnh {adjustmentRule.Name} cho cửa hàng {store.StoreName}",
                CreatedAt = DateTime.UtcNow
            };
            try
            {
                await _adjustmentRuleRepository.AddAsync(adjustmentRule);
                await _logRepository.AddAsync(log);
                return ServiceResult<string>.Success("Thêm thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error when add new adjustment rule");
                return ServiceResult<string>.Error("Lỗi khi thêm điều chỉnh mới");
            }
        }
    }
}
