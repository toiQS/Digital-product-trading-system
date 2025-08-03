namespace DPTS.Applications.Seller.Dtos.product
{
    public class ProductDetailDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public List<ImageDto> Images { get; set; } = new List<ImageDto>();
        public int TotalComplaints { get; set; }
        public decimal DiscountedPrice { get; set; }
        public decimal DiscountedValue { get; set; }
        public string SummaryFeature { get; set; } = string.Empty;
    }
    public class ImageDto
    {
        public string Id { get; set; }
        public string ImagePath { get; set; }
    }
}
