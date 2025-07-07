using System.ComponentModel.DataAnnotations;

namespace DPTS.Domains
{
    public enum ParticipantType
    {
        User,
        Store
    }

    public class Message
    {
        [Key]
        public string MessageId { get; set; } = string.Empty;

        public ParticipantType SenderType { get; set; }
        public string SenderId { get; set; } = string.Empty;

        public ParticipantType ReceiverType { get; set; }
        public string ReceiverId { get; set; } = string.Empty;

        public string Content { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsSystem { get; set; }
    }

}