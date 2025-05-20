using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Sys.Domain
{
    public class Product
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        public string SellerId { get; set; } = string.Empty;
        public string Title {  get; set; } = string.Empty;
        public StringBuilder Description {  get; set; } = new StringBuilder();
        public string Category {  get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        [Column("Create At")]
        public DateTime CreateAt { get; set; }
        [Column("Update At")]
        public DateTime UpdateAt { get; set; }
        [Column("Is Deleted")]
        public bool IsDeleted { get; set; }
        [Column("Is Circulated")]
        public bool IsCirculated { get; set; }
        public double Price { get; set; }
    }
}
