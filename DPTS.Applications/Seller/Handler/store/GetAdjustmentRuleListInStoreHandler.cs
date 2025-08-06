using DPTS.Applications.Seller.Dtos.store;
using DPTS.Applications.Seller.Query.store;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Seller.Handler.store
{
    public class GetAdjustmentRuleListInStoreHandler : IRequestHandler<GetAdjustmentRuleInStoreQuery, ServiceResult<AdjustmentRuleListInStoreDto>>
    {
        private readonly IAdjustmentRuleRepository _adjustmentRuleRepository;
        private readonly ILogger<GetAdjustmentRuleListInStoreHandler> _logger;
        private readonly IStoreRepository _storeRepository;

        public GetAdjustmentRuleListInStoreHandler(IAdjustmentRuleRepository adjustmentRuleRepository, ILogger<GetAdjustmentRuleListInStoreHandler> logger, IStoreRepository storeRepository)
        {
            _adjustmentRuleRepository = adjustmentRuleRepository;
            _logger = logger;
            _storeRepository = storeRepository;
        }

        public async Task<ServiceResult<AdjustmentRuleListInStoreDto>> Handle(GetAdjustmentRuleInStoreQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Handling GetAdjustmentRuleInStoreQuery for StoreId: {StoreId}", request.StoreId);
            var store = await _storeRepository.GetByIdAsync(request.StoreId);
            if( store == null)
            {
                _logger.LogError("Store not found with StoreId: {StoreId}", request.StoreId);
                return ServiceResult<AdjustmentRuleListInStoreDto>.Error("Không tìm thấy cửa hàng");
            }
            var adjustmentRules = await _adjustmentRuleRepository.GetAdjustmentRulesByStoreIdAsync(request.StoreId);
            var result = new AdjustmentRuleListInStoreDto
            {
                AdjustmentRules = adjustmentRules.Select(r => new AdjustmentRuleIndexListInStoreDto
                {
                    RuleId = r.RuleId,
                    Name = r.Name,
                    Description = r.Description,
                    MaxCap = r.MaxCap,
                    PerUserLimit = r.PerUserLimit,
                    UsageLimit = r.UsageLimit,
                    MinOrderAmount = r.MinOrderAmount,
                    IsPercentage = r.IsPercentage,
                    VoucherCode = r.VoucherCode,
                    From = r.From,
                    To = r.To,
                    Status = r.Status switch
                    {
                        Domains.RuleStatus.Active => "Active",
                        Domains.RuleStatus.Inactive => "Inactive",
                        Domains.RuleStatus.Expired => "Expired",
                        _ => "Unknown"
                    },
                }).ToList()
            };
            result.TotalCount = result.AdjustmentRules.Count;
            if(request.PageSize > 0 && request.PageIndex > 0)
            {
                result.AdjustmentRules = result.AdjustmentRules
                    .Skip((request.PageIndex - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();
            }
            return ServiceResult<AdjustmentRuleListInStoreDto>.Success(result);
        }
    }
}
