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
        [Column("action", TypeName = "varchar(512)")]
        public string Action { get; set; } = string.Empty;

        [Column("target_type")]
        public string? TargetType { get; set; }

        [Column("target_id")]
        public string? TargetId { get; set; }

        [Column("user_type")]
        public string? UserType { get; set; }

        [Column("ip_address")]
        public string? IpAddress { get; set; }

        [Column("user_agent")]
        public string? UserAgent { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User? User { get; set; }
    }
}