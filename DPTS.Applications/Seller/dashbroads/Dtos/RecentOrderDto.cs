namespace DPTS.Applications.Seller.dashbroads.Dtos
{
    public class RecentOrderDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Time { get; set; } = string.Empty;
    }
}
