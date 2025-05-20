using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class ProductReview
    {
        [Key]
        [Column("review_id")]
        public string ReviewId { get; set; } = string.Empty;

        [Column("product_id")]
        public string ProductId { get; set; } = string.Empty;

        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Column("rating")]
        public int? Rating { get; set; }

        [Column("comment")]
        public string? Comment { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Product Product { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }

}
