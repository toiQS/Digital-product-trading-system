using DPTS.Domains;

namespace DPTS.APIs.Models
{
    public class OrderModel : PagingModel
    {
        public string SellerId { get; set; } = string.Empty;
    }
    public class OrderOptionTimeModel
    {
        public string SellerId { get; set; } = string.Empty;
        public bool IsDay { get; set; }
        public bool IsWeek { get; set; }
        public bool IsMonth { get; set; }
        public bool IsYear { get; set; }
    }
    public class GetOrdersWithManyConditionModel : PagingModel
    {
        public DateTime From { get; set; }
        public DateTime To { get; set; }
        public string SellerId { get; set; } = string.Empty;
        public string Text { get; set; } = string.Empty;
        public EscrowStatus? Status { get; set; } = null;
    }
}