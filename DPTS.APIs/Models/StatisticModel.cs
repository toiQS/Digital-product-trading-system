namespace DPTS.APIs.Models
{
    public class StatisticModel
    {
    }
    public class StatisticOfSeller : PagingModel
    {
        public string SellerId { get; set; } = string.Empty;
    }
}
