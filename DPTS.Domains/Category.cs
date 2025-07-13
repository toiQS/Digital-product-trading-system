using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    [Table("category")]
    public class Category
    {
        public Category(
            string categoryName,
            string categoryIcon,
            int displayOrder,
            bool isFeatured)
        {
            CategoryId = Guid.NewGuid().ToString();
            CategoryName = categoryName;
            CategoryIcon = categoryIcon;
            DisplayOrder = displayOrder;
            IsFeatured = isFeatured;
            CreateAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        [Key]
        [Column("category_id")]
        public string CategoryId { get; init; }

        [Required]
        [Column("category_name")]
        public string CategoryName { get; init; }

        [Column("category_icon")]
        public string CategoryIcon { get; init; }

        [Column("create_at")]
        public DateTime CreateAt { get; init; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; init; }

        [Column("display_order")]
        public int DisplayOrder { get; init; }

        [Column("is_featured")]
        public bool IsFeatured { get; init; }

        // Navigation
        public virtual ICollection<Product> Products { get; init; } = new List<Product>();
        public virtual ICollection<AdjustmentRule> AdjustmentRules { get; init; } = new List<AdjustmentRule>();

        private Category() { }
    }
}
