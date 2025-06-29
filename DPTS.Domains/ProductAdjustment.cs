using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.TimeZoneInfo;

namespace DPTS.Domains
{
    public class ProductAdjustment
    {
        [Key]
        [Column("product_adjustment_id")]
        public string Id { get; set; } = string.Empty;

        [Required]
        [Column("product_id")]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        [Column("rule_id")]
        public string RuleId { get; set; } = string.Empty;

        public virtual Product Product { get; set; } = null!;
        public virtual AdjustmentRule Rule { get; set; } = null!;
    }
}