using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public enum ProductStatus
    {
        Unknown,
        Pending,
        Available,
        Blocked
    }

    [Table("product")]
    public class Product
    {
        private Product() { }

        public Product(
            string storeId,
            string productName,
            string? description,
            decimal originalPrice,
            string categoryId,
            string summaryFeature,
            ProductStatus status)
        {
            ProductId = Guid.NewGuid().ToString();
            StoreId = storeId;
            ProductName = productName;
            Description = description;
            OriginalPrice = originalPrice;
            CategoryId = categoryId;
            SummaryFeature = summaryFeature;
            Status = status;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        [Key]
        [Column("product_id")]
        public string ProductId { get; init; }

        [Required]
        [Column("store_id")]
        public string StoreId { get; init; }

        [Required]
        [Column("product_name")]
        public string ProductName { get; init; }

        [Column("description")]
        public string? Description { get; init; }

        [Required]
        [Column("original_price")]
        public decimal OriginalPrice { get; init; }

        [Required]
        [Column("category_id")]
        public string CategoryId { get; init; }

        [Column("summary_feature")]
        public string SummaryFeature { get; init; }

        [Column("status")]
        public ProductStatus Status { get; init; }

        [Column("created_at")]
        public DateTime CreatedAt { get; init; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; init; }

        public virtual Store Store { get; init; } = null!;
        public virtual Category Category { get; init; } = null!;
        public virtual ICollection<ProductAdjustment> ProductAdjustments { get; init; } = new List<ProductAdjustment>();
        public virtual ICollection<ProductImage> ProductImages { get; init; } = new List<ProductImage>();
        public virtual ICollection<ProductReview> ProductReviews { get; init; } = new List<ProductReview>();
        public virtual ICollection<CartItem> CartItems { get; init; } = new List<CartItem>();
        public virtual ICollection<OrderItem> OrderItems { get; init; } = new List<OrderItem>();
    }
}
