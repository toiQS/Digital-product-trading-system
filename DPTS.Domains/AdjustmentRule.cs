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

        [Column("is_percentage")]
        public bool IsPercentage { get; set; } = true;

        [Column("value")]
        public decimal Value { get; set; } = 0.0m;

        [Column("effective_from")]
        public DateTime? From { get; set; }

        [Column("effective_to")]
        public DateTime? To { get; set; }

        [Column("version")]
        public int Version { get; set; } = 1;

        [Column("is_default")]
        public bool IsDefaultForNewProducts { get; set; } = false;

        [Column("status")]
        public RuleStatus Status { get; set; } = RuleStatus.Active;

        public virtual ICollection<ProductAdjustment> ProductAdjustments { get; set; } = new List<ProductAdjustment>();
    }

    public enum AdjustmentType
    {
        Tax,
        Discount,
        PlatformFee
    }

    public enum RuleStatus
    {
        Active,
        Inactive,
        Expired
    }
}