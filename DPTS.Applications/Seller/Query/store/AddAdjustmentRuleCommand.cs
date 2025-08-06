using DPTS.Applications.Shareds;
using MediatR;

namespace DPTS.Applications.Seller.Query.store
{
    public class AddAdjustmentRuleCommand : IRequest<ServiceResult<string>>
    {
        public string UserId { get; set; }
        public string StoreId { get; set; }
        public AddAdjustmentRuleDto AdjustmentRule { get; set; } = new AddAdjustmentRuleDto();
    }
    public class AddAdjustmentRuleDto
    {
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public TargetLogicForStore TargetLogic { get; set; }
        public decimal? MinOrderAmount { get; set; }
        public bool IsPercentage { get; set; } = true;
        public decimal Value { get; set; } = 0.0m;
        public decimal? MaxCap { get; set; }
        public int? UsageLimit { get; set; }
        public int? PerUserLimit { get; set; }
        public RuleStatusForStore Status { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public string? VoucherCode { get; set; } = null;
    }


    public enum TargetLogicForStore
    {
        Auto,
        Voucher
    }

  
    public enum RuleStatusForStore
    {
        Active,
        Inactive,
        Expired
    }
}
