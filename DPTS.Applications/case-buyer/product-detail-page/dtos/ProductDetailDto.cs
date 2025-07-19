namespace DPTS.Applications.case_buyer.product_detail_page.dtos
{
    public class ProductDetailDto
    {
        public ProductOverviewDto ProductOverview { get; set; }
        public List<ProductImageIndexDto> ProductImages { get; set; } = new List<ProductImageIndexDto>();
        public CustomersFeelings CustomersFeelings { get; set; } = new CustomersFeelings();
    }
    public class ProductOverviewDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
        public decimal Discount { get; set; }
        public string StoreImage { get; set; }
        public string StoreName { get; set; }
        public string SummaryFeature { get; set; }
        public double OverallRating { get; set; }
        public int TotalReviews { get; set; }
        public string CategoryName { get; set; }
    }
    public class ProductImageIndexDto
    {
        public string ImageId { get; set; }
        public string ImageUrl { get; set; }
    }
    public class CustomersFeelings
    {
        public int Rate1AndOver { get; set; }
        public int Rate2AndOver { get; set; }
        public int Rate3AndOver { get; set; }
        public int Rate4AndOver { get; set; }
        public int Rate5AndOver { get; set; }
        public List<ProductReviewIndexDto> PositiveReviews { get; set; } = new List<ProductReviewIndexDto>();
    }
    public class ProductReviewIndexDto
    {
        public string ReviewId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string CustomerImage { get; set; }
        public string ReviewContent { get; set; }
        public double Rating { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
