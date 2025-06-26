using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class ProductReview
    {
        [Key]
        [Column("review_id")]
        public string ReviewId { get; set; } = string.Empty;

        [Required]
        [Column("product_id")]
        public string ProductId { get; set; } = string.Empty;

        [Required]
        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Column("rating_overall")]
        public int RatingOverall { get; set; } = 0;

        [Column("rating_quality")]
        public int RatingQuality { get; set; } = 0;

        [Column("rating_value")]
        public int RatingValue { get; set; } = 0;

        [Column("rating_usability")]
        public int RatingUsability { get; set; } = 0;

        [Column("review_title")]
        public string ReviewTitle { get; set; } = string.Empty;

        [Column("comment")]
        public string Comment { get; set; } = string.Empty;

        [Column("recommend_to_others")]
        public bool RecommendToOthers { get; set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("likes")]
        public int Likes { get; set; } = 0;

        [ForeignKey("ProductId")]
        public virtual Product Product { get; set; } = null!;

        [ForeignKey("UserId")]
        public virtual User User { get; set; } = null!;
    }
}
