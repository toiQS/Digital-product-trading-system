using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class Message
    {
        [Key]
        [Column("message_id")]
        public string MessageId { get; set; } = string.Empty;

        [Column("sender_id")]
        public string SenderId { get; set; } = string.Empty;

        [Column("receiver_id")]
        public string ReceiverId { get; set; } = string.Empty;

        [Column("order_id")]
        public string? OrderId { get; set; }

        [Required]
        [Column("content")]
        public string Content { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

       
        public virtual User Sender { get; set; } = null!;

        public virtual User Receiver { get; set; } = null!;

        public virtual Order? Order { get; set; }
    }
}
