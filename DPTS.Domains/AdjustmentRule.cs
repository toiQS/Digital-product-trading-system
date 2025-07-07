using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class AdjustmentRule
    {
        [Key]
        [Column("rule_id")]
        public string RuleId { get; set; } = string.Empty;

        [Column("name")]
        public string Name { get; set; } = string.Empty;

        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Column("type")]
        public AdjustmentType Type { get; set; }

        [Column("scope")]
        public AdjustmentScope Scope { get; set; }

        [Column("target_logic")]
        public TargetLogic TargetLogic { get; set; }

        [Column("source")]
        public AdjustmentSource Source { get; set; }

        [Column("is_percentage")]
        public bool IsPercentage { get; set; } = true;

        [Column("value")]
        public decimal Value { get; set; } = 0.0m;

        [Column("max_cap")]
        public decimal? MaxCap { get; set; }

        [Column("min_order_amount")]
        public decimal? MinOrderAmount { get; set; }

        [Column("voucher_code")]
        public string? VoucherCode { get; set; }

        [Column("usage_limit")]
        public int? UsageLimit { get; set; }

        [Column("per_user_limit")]
        public int? PerUserLimit { get; set; }

        [Column("effective_from")]
        public DateTime? From { get; set; }

        [Column("effective_to")]
        public DateTime? To { get; set; }

        [Column("status")]
        public RuleStatus Status { get; set; } = RuleStatus.Active;

        [Column("conditions_json")]
        public string? ConditionsJson { get; set; }

        public virtual ICollection<ProductAdjustment> ProductAdjustments { get; set; } = new List<ProductAdjustment>();
    }

    public enum AdjustmentType
    {
        Tax,
        Discount,
        PlatformFee
    }

    public enum AdjustmentScope
    {
        PerProduct,
        PerOrder
    }

    public enum TargetLogic
    {
        Auto,
        Voucher,
        Campaign
    }

    public enum AdjustmentSource
    {
        Platform,
        Seller,
        System
    }

    public enum RuleStatus
    {
        Active,
        Inactive,
        Expired
    }
}
