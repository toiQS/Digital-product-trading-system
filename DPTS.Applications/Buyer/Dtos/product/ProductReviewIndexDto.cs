﻿namespace DPTS.Applications.Buyer.Dtos.product
{
    public class ProductReviewIndexDto
    {
        public string ReviewId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string UserName { get; set; } = string.Empty;
        public string ImageUser { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public int RatingOverall { get; set; }
        public int Likes { get; set; }
        public string CreatedAt { get; set; } = string.Empty;
    }
}
