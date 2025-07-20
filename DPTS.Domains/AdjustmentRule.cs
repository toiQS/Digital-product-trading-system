using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.InteropServices;

namespace DPTS.Domains
{
    public enum AdjustmentType
    {
        Unknown,
        Tax,
        Discount,
        PlatformFee
    }

    public enum AdjustmentScope
    {
        Unknown,
        PerProduct,
        PerOrder,
        PerBuyer
    }

    public enum TargetLogic
    {
        Unknown,
        Auto,
        Voucher,
        Campaign
    }

    public enum AdjustmentSource
    {
        Unknown,    
        Platform,
        Store,
        System
    }

    public enum RuleStatus
    {
        Unknown,
        Active,
        Inactive,
        Expired
    }

    [Table("adjustment_rule")]
    public class AdjustmentRule
    {
      
        public AdjustmentRule(
            string personFirstId,
            string name,
            string description,
            AdjustmentType type,
            AdjustmentScope scope,
            TargetLogic targetLogic,
            AdjustmentSource source,
            bool isPercentage,
            decimal value,
            decimal? maxCap,
            decimal? minOrderAmount,
            string? voucherCode,
            int? usageLimit,
            int? perUserLimit,
            DateTime? from,
            DateTime? to,
            RuleStatus status,
            string? conditionsJson)
        {
            RuleId = Guid.NewGuid().ToString();
            PersonFirstId = personFirstId;
            Name = name;
            Description = description;
            Type = type;
            Scope = scope;
            TargetLogic = targetLogic;
            Source = source;
            IsPercentage = isPercentage;
            Value = value;
            MaxCap = maxCap;
            MinOrderAmount = minOrderAmount;
            VoucherCode = voucherCode;
            UsageLimit = usageLimit;
            PerUserLimit = perUserLimit;
            From = from;
            To = to;
            Status = status;
            ConditionsJson = conditionsJson;
        }

        [Key]
        [Column("rule_id")]
        public string RuleId { get; init; }
        [Column("personFirst_id")]
        public string PersonFirstId   {  get; set; }


        [Column("name")]
        public string Name { get; init; }

        [Column("description")]
        public string Description { get; init; }

        [Column("type")]
        public AdjustmentType Type { get; init; }

        [Column("scope")]
        public AdjustmentScope Scope { get; init; }

        [Column("target_logic")]
        public TargetLogic TargetLogic { get; init; }

        [Column("source")]
        public AdjustmentSource Source { get; init; }

        [Column("is_percentage")]
        public bool IsPercentage { get; init; }

        [Column("value")]
        public decimal Value { get; init; }

        [Column("max_cap")]
        public decimal? MaxCap { get; init; }

        [Column("min_order_amount")]
        public decimal? MinOrderAmount { get; init; }

        [Column("voucher_code")]
        public string? VoucherCode { get; init; }

        [Column("usage_limit")]
        public int? UsageLimit { get;set; }

        [Column("per_user_limit")]
        public int? PerUserLimit { get; init; }

        [Column("effective_from")]
        public DateTime? From { get; init; }

        [Column("effective_to")]
        public DateTime? To { get; init; }

        [Column("status")]
        public RuleStatus Status { get; init; }

        [Column("conditions_json")]
        public string? ConditionsJson { get; init; }

        public virtual ICollection<ProductAdjustment> ProductAdjustments { get; init; } = new List<ProductAdjustment>();

        private AdjustmentRule() { }
    }
}
