namespace DPTS.Applications.Buyer.Dtos.review
{
    public class ProductReviewDetailDto
    {
        public string ReviewId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
        public string AvatarUrl { get; set; } = string.Empty;
        public string Title { get; set; }
        public string Comment { get; set; } = string.Empty;
        public double RatingOverall { get; set; } = 0;
        public int RatingQuality { get; set; } = 0;
        public int RatingValue { get; set; } = 0;
        public int RatingUsability { get; set; } = 0;
        public bool RecommendToOthers { get; set; } = false;
        
    }
    
}
