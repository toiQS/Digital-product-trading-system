namespace DPTS.Applications.Seller.dashbroads.Dtos
{
    public class RevenueChartDto
    {
        public string Label { get; set; } = string.Empty;  // e.g. T2, T3, T4...
        public decimal Revenue { get; set; }
    }
}
