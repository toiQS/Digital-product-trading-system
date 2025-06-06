namespace DPTS.Applications.Dtos
{
    public class ProductIndexDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int QuantitySelled { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public double Rating { get; set; } = 0.0f;
    }
    public class ProductDetailDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public double Rating { get; set; } = 0.0f;
        public List<ImageIndexDto> Images { get; set; } = new List<ImageIndexDto>();
        public List<ProductReviewIndexDto> Reviews { get; set; } = new List<ProductReviewIndexDto>();
    }
    public class ProductReviewIndexDto
    {
        public string ProductReviewId { get; set; } = string.Empty;
        public string BuyerName { get; set; } = string.Empty;
        public string Comment { get; set; } = string.Empty;
        public float Rating { get; set; } = 0.0f;
        public int Likes { get; set; } = 0;
        public DateTime CreatedAt { get; set; } 
    }
}
