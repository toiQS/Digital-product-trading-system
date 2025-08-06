using DPTS.Applications.Seller.Query.store;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.Handler.store
{
    public class UpdateAdjustmentRuleHandler : IRequestHandler<UpdateAdjustmentRuleCommand, ServiceResult<string>>
    {
        private readonly IUserRepository _userRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IAdjustmentRuleRepository _adjustmentRuleRepository;
        private readonly ILogRepository _logRepository;
        private readonly ILogger<UpdateAdjustmentRuleHandler> _logger;

        public UpdateAdjustmentRuleHandler(IUserRepository userRepository, IStoreRepository storeRepository, IAdjustmentRuleRepository adjustmentRuleRepository, ILogRepository logRepository, ILogger<UpdateAdjustmentRuleHandler> logger)
        {
            _userRepository = userRepository;
            _storeRepository = storeRepository;
            _adjustmentRuleRepository = adjustmentRuleRepository;
            _logRepository = logRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<string>> Handle(UpdateAdjustmentRuleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling UpdateAdjustmentRuleCommand for UserId: {UserId}, StoreId: {StoreId}, AdjustmentRuleId: {AdjustmentRuleId}",
                request.UserId, request.StoreId, request.AdjustmentRuleId);
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
            var adjustmentRule = await _adjustmentRuleRepository.GetByIdAsync(request.AdjustmentRuleId);
            if (adjustmentRule == null)
            {
                _logger.LogError("AdjustmentRule not found with AdjustmentRuleId: {AdjustmentRuleId}", request.AdjustmentRuleId);
                return ServiceResult<string>.Error("Không tìm thấy quy tắc điều chỉnh");
            }
            if (adjustmentRule.SourceId != request.StoreId)
            {
                _logger.LogError("AdjustmentRule with AdjustmentRuleId: {AdjustmentRuleId} does not belong to UserId: {UserId}",
                    request.AdjustmentRuleId, request.UserId);
                return ServiceResult<string>.Error("Quy tắc điều chỉnh không thuộc về người dùng này");
            }
            var log = new Log
            {
                UserId = request.UserId,
                Action = "UpdateAdjustmentRule",
                Description = $"Cập nhật quy tắc điều chỉnh {request.AdjustmentRuleId} cho cửa hàng {request.StoreId}",
                CreatedAt = DateTime.UtcNow
            };
            try
            {
                adjustmentRule.Name = request.AdjustmentRule.Name ?? adjustmentRule.Name;
                adjustmentRule.Description = request.AdjustmentRule.Description ?? adjustmentRule.Description;
                adjustmentRule.TargetLogic = request.AdjustmentRule.TargetLogic switch
                {
                    TargetLogicForStore.Auto => Domains.TargetLogic.Auto,
                    TargetLogicForStore.Voucher => Domains.TargetLogic.Voucher,
                    _ => throw new ArgumentOutOfRangeException(nameof(request.AdjustmentRule.TargetLogic), "Invalid target logic")
                };
                adjustmentRule.MinOrderAmount = request.AdjustmentRule.MinOrderAmount ?? adjustmentRule.MinOrderAmount;
                adjustmentRule.IsPercentage = request.AdjustmentRule.IsPercentage;
                adjustmentRule.Value = request.AdjustmentRule.Value;
                adjustmentRule.MaxCap = request.AdjustmentRule.MaxCap ?? adjustmentRule.MaxCap;
                adjustmentRule.UsageLimit = request.AdjustmentRule.UsageLimit ?? adjustmentRule.UsageLimit;
                adjustmentRule.PerUserLimit = request.AdjustmentRule.PerUserLimit ?? adjustmentRule.PerUserLimit;
                adjustmentRule.VoucherCode = request.AdjustmentRule.VoucherCode ?? adjustmentRule.VoucherCode;
                adjustmentRule.Status = request.AdjustmentRule.Status switch
                {
                    RuleStatusForStore.Active => Domains.RuleStatus.Active,
                    RuleStatusForStore.Inactive => Domains.RuleStatus.Inactive,
                    RuleStatusForStore.Expired => Domains.RuleStatus.Expired,
                    _ => throw new ArgumentOutOfRangeException(nameof(request.AdjustmentRule.Status), "Invalid rule status")
                };
                adjustmentRule.From = request.AdjustmentRule.From ?? adjustmentRule.From;
                adjustmentRule.To = request.AdjustmentRule.To ?? adjustmentRule.To;

                await _adjustmentRuleRepository.UpdateAsync(adjustmentRule);
                await _logRepository.AddAsync(log);
                return ServiceResult<string>.Success("Cập nhật thành công");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating AdjustmentRule with AdjustmentRuleId: {AdjustmentRuleId}", request.AdjustmentRuleId);
                return ServiceResult<string>.Error("Cập nhật quy tắc điều chỉnh thất bại");
            }
        }
    }
}
