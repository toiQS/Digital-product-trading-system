using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    [Table("product_review")]
    public class ProductReview
    {
        private ProductReview() { } // For EF

        public ProductReview(
            string productId,
            string userId,
            int ratingQuality,
            int ratingValue,
            int ratingUsability,
            string reviewTitle,
            string comment,
            bool recommendToOthers)
        {
            ReviewId = Guid.NewGuid().ToString();
            ProductId = productId;
            UserId = userId;
            RatingQuality = ratingQuality;
            RatingValue = ratingValue;
            RatingUsability = ratingUsability;
            RatingOverall = Math.Round((double)(ratingQuality + ratingValue + ratingUsability) / 3, 1);
            ReviewTitle = reviewTitle;
            Comment = comment;
            RecommendToOthers = recommendToOthers;
            CreatedAt = DateTime.UtcNow;
            Likes = 0;
            ProductReviewImages = new List<ProductReviewImage>();
        }

        [Key]
        [Column("review_id")]
        public string ReviewId { get; init; }

        [Required]
        [Column("product_id")]
        public string ProductId { get; init; }

        [Required]
        [Column("user_id")]
        public string UserId { get; init; }

        [Column("rating_overall")]
        public double RatingOverall { get; init; }

        [Column("rating_quality")]
        public int RatingQuality { get; init; }

        [Column("rating_value")]
        public int RatingValue { get; init; }

        [Column("rating_usability")]
        public int RatingUsability { get; init; }

        [Column("review_title")]
        public string ReviewTitle { get; init; }

        [Column("comment")]
        public string Comment { get; init; }

        [Column("recommend_to_others")]
        public bool RecommendToOthers { get; init; }

        [Column("created_at")]
        public DateTime CreatedAt { get; init; }

        [Column("likes")]
        public int Likes { get; set; }

        public virtual Product Product { get; init; } = null!;
        public virtual User User { get; init; } = null!;
        public virtual ICollection<ProductReviewImage> ProductReviewImages { get; init; } = new List<ProductReviewImage>();
    }
}
