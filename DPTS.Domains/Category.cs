using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class Category
    {
        [Key]
        [Column("category_id")]
        public string CategoryId { get; set; } = string.Empty;

        [Required]
        [Column("category_name")]
        public string CategoryName { get; set; } = string.Empty;

        [Column("category_icon")]
        public string CategoryIcon { get; set; } = string.Empty;
        [Column("description")]
        public string Description { get; set; }

        [Column("create_at")]
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("display_order")]
        public int DisplayOrder { get; set; } = 0;

        [Column("is_featured")]
        public bool IsFeatured { get; set; } = false;

        // Navigation
        public virtual ICollection<Product> Products { get; set; } = new List<Product>();
    }
}
