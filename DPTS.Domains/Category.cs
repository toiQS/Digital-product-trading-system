using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    [Table("category")]
    public class Category
    {
        private Category() { } // For EF

        public Category(string categoryName, string categoryIcon, int displayOrder, bool isFeatured)
        {
            CategoryId = Guid.NewGuid().ToString();
            CategoryName = categoryName;
            CategoryIcon = categoryIcon;
            DisplayOrder = displayOrder;
            IsFeatured = isFeatured;
            IsActive = true;
            CreateAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            Products = new List<Product>();
            AdjustmentRules = new List<AdjustmentRule>();
        }

        [Key]
        [Column("category_id")]
        public string CategoryId { get; init; }

        [Required]
        [Column("category_name")]
        public string CategoryName { get; private set; }

        [Column("category_icon")]
        public string CategoryIcon { get; private set; }

        [Column("create_at")]
        public DateTime CreateAt { get; init; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [Column("display_order")]
        public int DisplayOrder { get; private set; }

        [Column("is_featured")]
        public bool IsFeatured { get; private set; }

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        // Navigation
        public virtual ICollection<Product> Products { get; init; }
        public virtual ICollection<AdjustmentRule> AdjustmentRules { get; init; }

        // Domain logic
        public void UpdateBasicInfo(string name, string icon, int order, bool isFeatured)
        {
            CategoryName = name;
            CategoryIcon = icon;
            DisplayOrder = order;
            IsFeatured = isFeatured;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Deactivate()
        {
            IsActive = false;
            UpdatedAt = DateTime.UtcNow;
        }

        public void Activate()
        {
            IsActive = true;
            UpdatedAt = DateTime.UtcNow;
        }
    }
}
