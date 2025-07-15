using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class Complaint
    {
        [Key]
        [Column("complaint_id")]
        public string ComplaintId { get; set; } = string.Empty;

        [Column("escrow_id")]
        public string EscrowId { get; set; } = string.Empty ;
        [Column("product_id")]
        public string ProductId { get; set; } = string.Empty;
        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Column("status")]
        public ComplaintStatus Status { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        public List<ComplaintImage> Images { get; set; } = new List<ComplaintImage>();

        public virtual Escrow Escrow { get; set; } = null!;
        public virtual User User { get; set; } = null!;
        public virtual Product Product { get; set; } = null!;
    }
    public enum ComplaintStatus
    {
        Unknown,
        Pending,
        Resolved
    }

}
