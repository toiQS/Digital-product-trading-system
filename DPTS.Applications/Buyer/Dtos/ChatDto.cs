namespace DPTS.Applications.Buyer.Dtos
{
    public class ChatDto
    {
        public bool IsOwnMessage { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime ReceiveAt { get; set; }
    }
}
