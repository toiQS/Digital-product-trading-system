namespace DPTS.Applications.Buyer.Dtos.chat
{
    public class ChatIndexListDto
    {
        public string ContentWith { get; set; } = string.Empty;
        public List<ChatDto> Messages { get; set; } = new List<ChatDto>();
    }
}
