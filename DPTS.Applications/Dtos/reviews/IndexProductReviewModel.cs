namespace DPTS.Applications.Dtos.reviews
{
    public class IndexProductReviewModel
    {
        public string ProductReviewId { get; set; } = string.Empty;
        public string BuyerName { get; set; } = string.Empty;
        public string Content { get; set; } = string.Empty;
        public float Rating { get; set; } = 0.0f;
        public int Likes { get; set; } = 0;
    }
}
