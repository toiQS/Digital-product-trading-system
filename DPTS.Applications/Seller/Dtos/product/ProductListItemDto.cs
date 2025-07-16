namespace DPTS.Applications.Seller.Dtos.product
{
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
