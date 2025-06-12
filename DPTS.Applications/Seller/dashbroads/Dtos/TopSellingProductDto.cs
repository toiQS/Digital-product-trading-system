namespace DPTS.Applications.Seller.dashbroads.Dtos
{
    public class TopSellingProductDto
    {
        public string ProductName { get; set; } = string.Empty;
        public string CategoryCode { get; set; } = string.Empty; // UI, PH, etc.
        public int QuantitySold { get; set; }
        public decimal TotalRevenue { get; set; }
    }
}
