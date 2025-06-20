﻿using System.ComponentModel.DataAnnotations;
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

        [Column("rating")]
        public int Rating { get; set; } = 0;

        [Column("comment")]
        public string Comment { get; set; } = string.Empty;

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
