namespace DPTS.Applications.Buyer.Dtos
{
    public class ProductDetailDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public double RatingOverall { get; set; }
        public int CountReviews { get; set; }
        public string StoreImage { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal Discount { get; set; }
        public string SummaryFeature { get; set; } = string.Empty;

        public List<string> ProductImage { get; set; } = new();

        // Body
        public string Description { get; set; } = string.Empty;
        public decimal Vote1 { get; set; }
        public decimal Vote2 { get; set; }
        public decimal Vote3 { get; set; }
        public decimal Vote4 { get; set; }
        public decimal Vote5 { get; set; }
        public List<ProductReviewIndexDto> ProductReviews { get; set; } = new List<ProductReviewIndexDto>();
        public List<ProductIndexDto> ProductSuggest { get; set; } = new List<ProductIndexDto>();
    }
}
