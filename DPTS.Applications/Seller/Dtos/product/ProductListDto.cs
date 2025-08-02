namespace DPTS.Applications.Seller.Dtos.product
{
    public class ProductListDto
    {
        public int Total { get; set; }
        public int Count { get; set; }
        public List<ProductListItemDto> Items { get; set; } = new List<ProductListItemDto>();
    }
    public class ProductListItemDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty;
        public int QuantitySelled { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; } = string.Empty;
        public double RatingOverall { get; set; }
    }
}
