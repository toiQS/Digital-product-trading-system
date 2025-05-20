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
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [Column("price")]
        public decimal Price { get; set; }

        [Column("category")]
        public string? Category { get; set; }

        [Column("status")]
        public string Status { get; set; } = "pending";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("is_circulate")]
        public bool IsCirculate { get; set; }

        public virtual User Seller { get; set; } = null!;
        public virtual ICollection<ProductReview> Reviews { get; set; } = new List<ProductReview>();
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
    }

}
