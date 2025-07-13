using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    [Table("product_image")]
    public class ProductImage
    {
        private ProductImage() { } // For EF

        public ProductImage(string productId, string imagePath, bool isPrimary)
        {
            ImageId = Guid.NewGuid().ToString();
            ProductId = productId;
            ImagePath = imagePath;
            IsPrimary = isPrimary;
            CreatedAt = DateTime.UtcNow;
        }

        [Key]
        [Column("image_id")]
        public string ImageId { get; init; }

        [Required]
        [Column("product_id")]
        public string ProductId { get; init; }

        [Required]
        [Column("image_path")]
        public string ImagePath { get; init; }

        [Column("is_primary")]
        public bool IsPrimary { get; init; }

        [Column("created_at")]
        public DateTime CreatedAt { get; init; }

        public virtual Product Product { get; init; } = null!;
    }
}
