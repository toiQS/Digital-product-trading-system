using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class ComplaintImage
    {
        [Key]
        [Column("image_id")]
        public string ImageId { get; set; } = string.Empty;

        [Required]
        [Column("complaint_id")]
        public string ComplaintId { get; set; } = string.Empty;

        [Required]
        [Column("image_path")]
        public string ImagePath { get; set; } = string.Empty;

        [ForeignKey("ComplaintId")]
        public virtual Complaint Complaint { get; set; } = null!;

        [Column("create_at")]
        public DateTime CreateAt { get; set; } = DateTime.UtcNow;
    }
}
