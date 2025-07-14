using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public enum ReceiverType
    {
        User,
        Store
    }

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

        [Column("is_read")]
        public bool IsRead { get; private set; } = false;

        [Column("created_at")]
        public DateTime CreatedAt { get; init; }

        [Column("read_at")]
        public DateTime? ReadAt { get; private set; }

        public void MarkAsRead()
        {
            IsRead = true;
            ReadAt = DateTime.UtcNow;
        }
    }
}
