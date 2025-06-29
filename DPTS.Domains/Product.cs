using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class Product
    {
        [Key]
        [Column("product_id")]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        [Column("store_id")]
        public string StoreId { get; set; } = string.Empty;

        [Required]
        [Column("product_name")]
        public string ProductName { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [Column("original_price")]
        public decimal OriginalPrice { get; set; }

        [Required]
        [Column("category_id")]
        public string CategoryId { get; set; } = string.Empty;

        [Column("summary")]
        public string Summary { get; set; } = string.Empty;

        [Column("status")]
        public ProductStatus Status { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual Store Store { get; set; } = null!;
        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<ProductAdjustment> ProductAdjustments { get; set; } = new List<ProductAdjustment>();
    }

    public enum ProductStatus
    {
        Unknown,
        Pending,
        Available,
        Blocked
    }
}
