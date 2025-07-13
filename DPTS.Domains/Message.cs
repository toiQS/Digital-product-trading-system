using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public enum ParticipantType
    {
        User,
        Store
    }

    [Table("message")]
    public class Message
    {
        public Message(
            ParticipantType senderType,
            string senderId,
            ParticipantType receiverType,
            string receiverId,
            string content)
        {
            MessageId = Guid.NewGuid().ToString();
            SenderType = senderType;
            SenderId = senderId;
            ReceiverType = receiverType;
            ReceiverId = receiverId;
            Content = content;
            CreatedAt = DateTime.UtcNow;
        }

        [Key]
        [Column("message_id")]
        public string MessageId { get; init; }

        [Column("sender_type")]
        public ParticipantType SenderType { get; init; }

        [Column("sender_id")]
        public string SenderId { get; init; }

        [Column("receiver_type")]
        public ParticipantType ReceiverType { get; init; }

        [Column("receiver_id")]
        public string ReceiverId { get; init; }

        [Column("content")]
        public string Content { get; init; }

        [Column("created_at")]
        public DateTime CreatedAt { get; init; }
        private Message() { }
    }
}
