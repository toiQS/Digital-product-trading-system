using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    [Table("product_review_image")]
    public class ProductReviewImage
    {
        private ProductReviewImage() { } // For EF

        public ProductReviewImage(string productReviewId, string imagePath)
        {
            ProductReviewImageId = Guid.NewGuid().ToString();
            ProductReviewId = productReviewId;
            ProductReviewImagePath = imagePath;
        }

        [Key]
        [Column("product_review_image_id")]
        public string ProductReviewImageId { get; init; }

        [Required]
        [Column("product_review_id")]
        public string ProductReviewId { get; init; }

        [Required]
        [Column("product_review_image_path")]
        public string ProductReviewImagePath { get; init; }

        public virtual ProductReview ProductReview { get; init; } = null!;
    }
}
