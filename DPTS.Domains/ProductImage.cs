using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class ProductImage
    {
        [Key]
        [Column("image_id")]
        public string ImageId { get; set; } = string.Empty;

        [Required]
        [Column("product_id")]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        [Column("image_path")]
        public string ImagePath { get; set; } = string.Empty;

        [Column("is_primary")]
        public bool IsPrimary { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
    }

}
