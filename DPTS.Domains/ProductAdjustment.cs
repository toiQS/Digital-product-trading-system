using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    [Table("product_adjustment")]
    public class ProductAdjustment
    {
        private ProductAdjustment() { } // For EF

        public ProductAdjustment(string productId, string ruleId)
        {
            Id = Guid.NewGuid().ToString();
            ProductId = productId;
            RuleId = ruleId;
        }

        [Key]
        [Column("product_adjustment_id")]
        public string Id { get; init; }

        [Required]
        [Column("product_id")]
        public string ProductId { get; init; }

        [Required]
        [Column("rule_id")]
        public string RuleId { get; init; }

        public virtual Product Product { get; init; } = null!;
        public virtual AdjustmentRule Rule { get; init; } = null!;
    }
}
