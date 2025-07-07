namespace DPTS.Applications.Buyer.Dtos.chat
{
    public class ChatDto
    {
        public bool IsOwnMessage { get; set; }
        public string Content { get; set; } = string.Empty;
        public DateTime ReceiveAt { get; set; }
    }
}
