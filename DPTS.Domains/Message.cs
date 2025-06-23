using DPTS.Domains;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Message
{
    [Key]
    [Column("message_id")]
    public string MessageId { get; set; } = string.Empty;
    [Column("sender_user_id")]
    public string? SenderUserId { get; set; }
    [Column("sender_store_id")]
    public string? SenderStoreId { get; set; }
    [Column("receiver_user_id")]
    public string? ReceiverUserId { get; set; }
    [Column("receiver_store_id")]
    public string? ReceiverStoreId { get; set; }

    [Required]
    [Column("content")]
    public string Content { get; set; } = string.Empty;
    [Column("create_at")]
    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    [Column("is_system")]
    public bool IsSystem { get; set; }

    [ForeignKey("SenderUserId")]
    public virtual User? SenderUser { get; set; }

    [ForeignKey("SenderStoreId")]
    public virtual Store? SenderStore { get; set; }

    [ForeignKey("ReceiverUserId")]
    public virtual User? ReceiverUser { get; set; }

    [ForeignKey("ReceiverStoreId")]
    public virtual Store? ReceiverStore { get; set; }
}
