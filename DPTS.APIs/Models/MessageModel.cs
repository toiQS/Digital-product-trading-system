namespace DPTS.APIs.Models
{
    public class MessageModel
    {
    }
    public class RecentMessageIndexModel : PagingModel
    {
        public string SellerId { get; set; } = string.Empty;
    }
}
