using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Sys.Domain
{
    [Table("Product Review")]
     public class ProductReview
    {
        [Key]
        public string Id { get; set; } = string.Empty;
        [Column("Product Id")]
        public string ProductId { get; set; } = string.Empty;
        [Column("User Id")]
        public string UserId { get; set; } = string.Empty ;
        public int Rating { get; set; }
        public string Comment { get; set; } = string.Empty;
        [Column("Create At")]
        public DateTime CreateAt { get; set; }

        //ForeignKey
        public virtual User User { get; set; }
        public virtual Product Product { get; set; }
    }
}
