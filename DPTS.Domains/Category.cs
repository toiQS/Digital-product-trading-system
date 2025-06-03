using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class Category
    {
        [Key]
        [Column("category_id")]
        public string CategoryId {  get; set; } = string.Empty;
        [Column("category_name")]
        public string CategoryName { get; set; } = string.Empty;
        [Column("category_icon")]
        public string CategoryIcon { get; set; } = string.Empty;
        [Column("create_at")]
        public DateTime CreateAt { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
