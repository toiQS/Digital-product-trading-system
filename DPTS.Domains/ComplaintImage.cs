using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    [Table("complaint_image")]
    public class ComplaintImage
    {
        private ComplaintImage() { } // For EF

        public ComplaintImage(string complaintId, string imagePath)
        {
            ImageId = Guid.NewGuid().ToString();
            ComplaintId = complaintId;
            ImagePath = imagePath;
            CreatedAt = DateTime.UtcNow;
        }

        [Key]
        [Column("image_id")]
        public string ImageId { get; init; }

        [Required]
        [Column("complaint_id")]
        public string ComplaintId { get; init; }

        [Required]
        [Column("image_path")]
        public string ImagePath { get; init; }

        [Column("created_at")]
        public DateTime CreatedAt { get; init; }

        [ForeignKey("ComplaintId")]
        public virtual Complaint Complaint { get; init; } = null!;
    }
}
