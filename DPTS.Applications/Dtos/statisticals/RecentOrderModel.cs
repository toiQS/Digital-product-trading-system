using DPTS.Domains;

namespace DPTS.Applications.Dtos.statisticals
{
    public class RecentOrderModel
    {
        public string ImagePath { get; set; } = string.Empty;
        public string BuyerName { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public StatusEntity Status { get; set; }
        public decimal Amount    { get; set; }
    }
}
