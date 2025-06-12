namespace DPTS.Applications.Seller.dashbroads.Dtos
{
    public class RecentMessageDto
    {
        public string CustomerName { get; set; } = string.Empty;
        public string ContentSnippet { get; set; } = string.Empty;
        public string TimeAgo { get; set; } = string.Empty; // "Hôm qua", "12/05", etc.
    }
}
