using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    [Table("complaint")]
    public class Complaint
    {
        private Complaint() { } // For EF

        public Complaint(string orderId, string itemId, string userId, string title, string description)
        {
            ComplaintId = Guid.NewGuid().ToString();
            OrderId = orderId;
            ItemId = itemId;
            UserId = userId;
            Title = title;
            Description = description;
            Status = ComplaintStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            Images = new List<ComplaintImage>();
        }

        [Key]
        [Column("complaint_id")]
        public string ComplaintId { get; init; }

        [Required]
        [Column("order_id")]
        public string OrderId { get; init; }

        [Required]
        [Column("order_item_id")]
        public string ItemId { get; init; }

        [Required]
        [Column("user_id")]
        public string UserId { get; init; }

        [Required]
        [Column("title")]
        public string Title { get; private set; }

        [Required]
        [Column("description")]
        public string Description { get; private set; }

        [Column("status")]
        public ComplaintStatus Status { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; init; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        // Navigation
        public virtual Order Order { get; init; } = null!;
        public virtual OrderItem OrderItem { get; init; } = null!;
        public virtual User User { get; init; } = null!;
        public virtual List<ComplaintImage> Images { get; init; }

        // Domain logic
        public void Resolve()
        {
            Status = ComplaintStatus.Resolved;
            UpdatedAt = DateTime.UtcNow;
        }

        public void UpdateContent(string title, string description)
        {
            Title = title;
            Description = description;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public enum ComplaintStatus
    {
        Unknown,
        Pending,
        Resolved
    }
}
