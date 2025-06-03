using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class Log
    {
        [Key]
        [Column("log_id")]
        public string LogId { get; set; } = string.Empty;

        [Column("user_id")]
        public string? UserId { get; set; }

        [Required]
        [Column("action")]
        public string Action { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User? User { get; set; }
    }

}
