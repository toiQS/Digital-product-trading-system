namespace DPTS.Applications.Seller.dashbroads.Dtos
{
    public class SellerDashboardDto
    {
        public decimal TotalRevenue { get; set; }
        public int TotalOrders { get; set; }
        public int TotalProducts { get; set; }
        public double AverageRating { get; set; }

        public List<RevenueChartDto> RevenueChart { get; set; } = new List<RevenueChartDto>();
        public List<TopSellingProductDto> TopSellingProducts { get; set; } = new List<TopSellingProductDto>();
        public List<RecentOrderDto> RecentOrders { get; set; } = new List<RecentOrderDto>();
        public List<RecentMessageDto> RecentMessages { get; set; } = new List<RecentMessageDto>();
    }
}
