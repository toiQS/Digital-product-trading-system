using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.AdjustmentRules
{
    public interface IAdjustmentRuleQuery
    {
        Task<AdjustmentRule?> GetByIdAsync(string ruleId, bool includeProductAdjustments = false);

        Task<List<AdjustmentRule>> GetActiveRulesAsync(DateTime asOf);

        Task<List<AdjustmentRule>> GetApplicableRulesAsync(
            AdjustmentType type = AdjustmentType.Unknown,
            string? sellerId = null,
            decimal orderAmount = 0m,
            DateTime? asOf = null);

        Task<AdjustmentRule?> GetByVoucherCodeAsync(
            string code,
            DateTime? asOf = null,
            bool includeProductAdjustments = false);

        // Kiểm tra giới hạn sử dụng voucher tổng và theo người dùng
        Task<bool> IsVoucherUsageAllowedAsync(string ruleId = default!,
                                              string userId = null!);

        Task<List<AdjustmentRule>> GetRulesByScopeAsync(
            AdjustmentScope scope = AdjustmentScope.Unknown,
            DateTime? asOf = null);

        Task<List<AdjustmentRule>> GetRulesBySourceAsync(
            AdjustmentSource source = AdjustmentSource.Unknown,
            string sellerId = null!);

        // Core filter-based retrievals
        Task<AdjustmentRule?> OGetAsync(OGetRecord options);
        Task<List<AdjustmentRule>> OGetsAsync(OGetsRecord options);
    }


    public record AdjustmentRuleFilterBase
    {
        public string? RuleId { get; init; }
        public string? SellerId { get; init; }
        public AdjustmentType? Type { get; init; }
        public AdjustmentScope? Scope { get; init; }
        public TargetLogic? TargetLogic { get; init; }
        public RuleStatus? Status { get; init; }
        public DateTime? AsOf { get; init; } // thời điểm xét hiệu lực
    }


    public record OGetRecord : AdjustmentRuleFilterBase
    {
        public bool? IsPercentage { get; init; }
        public decimal? Value { get; init; }
        public decimal? MaxCap { get; init; }
        public decimal? MinOrderAmount { get; init; }
        public string? VoucherCode { get; init; }
        public int? UsageLimit { get; init; }
        public int? PerUserLimit { get; init; }
        public DateTime? From { get; init; }
        public DateTime? To { get; init; }

        public bool IncludeProductAdjustments { get; init; } = false;
    }

    // Simpler: for list results
    public record OGetsRecord : AdjustmentRuleFilterBase
    {
        public bool IncludeProductAdjustments { get; init; } = false;
    }
}
