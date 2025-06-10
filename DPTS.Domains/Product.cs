using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace DPTS.Domains
{
    public class Product
    {
        [Key]
        [Column("product_id")]
        public string ProductId { get; set; } = string.Empty;

        [Column("seller_id")]
        public string SellerId { get; set; } = string.Empty;

        [Required]
        [Column("product_name")]
        public string ProductName { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [Column("price")]
        public double Price { get; set; }

        [Column("category")]
        public string CategoryId { get; set; }= string.Empty;   

        [Column("status")]
        public ProductStatus Status { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

       
        public virtual User Seller { get; set; } = null!;
        public virtual Category Category { get; set; } = null!;
        public virtual ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
        public virtual ICollection<ProductImage> Images { get; set; } = new List<ProductImage>();
        public virtual ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
    }
    public enum ProductStatus
    {
        Unknown,
        Pending,
        Available,
        Blocked
    }
    
}
