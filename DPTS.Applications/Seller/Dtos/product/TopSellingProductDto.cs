namespace DPTS.Applications.Seller.Dtos.product
{
    public class TopSellingProductDto
    {
        public string ProductName { get; set; } = string.Empty;
        public int OrderCount { get; set; }
        public decimal Revenue { get; set; }
        public string ImageUrl { get; set; } = string.Empty ;
        public string ProductId { get; set; } = string.Empty;
    }

}
