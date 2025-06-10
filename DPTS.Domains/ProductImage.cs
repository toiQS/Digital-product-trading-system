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
        public string ProductId { get; set; } = string.Empty ;

        [Required]
        [Column("image_path")]
        public string ImagePath { get; set; } = string.Empty;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;
        [Column("create_at")]
        public DateTime CreateAt { get; set; }
    }
}
