using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class Complaint
    {
        [Key]
        [Column("complaint_id")]
        public string ComplaintId { get; set; } = string.Empty;

        [Column("order_id")]
        public string OrderId { get; set; } = string.Empty ;

        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Column("title")]
        public string Title { get; set; } = string.Empty;

        [Required]
        [Column("description")]
        public string Description { get; set; } = string.Empty;

        [Column("status")]
        public string Status { get; set; } = "open";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual Order Order { get; set; } = null!;
        public virtual User User { get; set; } = null!;
    }

}
