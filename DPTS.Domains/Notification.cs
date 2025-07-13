using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    [Table("notification")]
    public class Notification
    {
        private Notification() { } // For EF

        public Notification(string receiverId, ReceiverType receiverType, string context)
        {
            NotificationId = Guid.NewGuid().ToString();
            ReceiverId = receiverId;
            ReceiverType = receiverType;
            Context = context;
            CreatedAt = DateTime.UtcNow;
        }

        [Key]
        [Column("notification_id")]
        public string NotificationId { get; init; }

        [Required]
        [Column("receiver_id")]
        public string ReceiverId { get; init; }

        [Required]
        [Column("receiver_type")]
        public ReceiverType ReceiverType { get; init; }

        [Required]
        [Column("context")]
        public string Context { get; init; }

        [Column("created_at")]
        public DateTime CreatedAt { get; init; }
    }

    public enum ReceiverType
    {
        User,
        Store
    }
}
