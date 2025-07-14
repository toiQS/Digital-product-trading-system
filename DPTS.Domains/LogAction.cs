using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    [Table("log_action")]
    public class LogAction
    {
        private LogAction() { } // For EF

        public LogAction(
            string? userId,
            string actionName,
            string actionDescription,
            string? targetType,
            string? targetId)
        {
            LogActionId = Guid.NewGuid().ToString();
            UserId = userId;
            ActionName = actionName;
            ActionDescription = actionDescription;
            TargetType = targetType;
            TargetId = targetId;
            CreatedAt = DateTime.UtcNow;
        }

        [Key]
        [Column("log_action_id")]
        public string LogActionId { get; init; }

        [Column("user_id")]
        public string? UserId { get; init; }

        [Required]
        [Column("action_name")]
        public string ActionName { get; init; }

        [Column("action_description")]
        public string ActionDescription { get; init; }

        [Column("target_type")]
        public string? TargetType { get; init; }

        [Column("target_id")]
        public string? TargetId { get; init; }

        [Column("created_at")]
        public DateTime CreatedAt { get; init; }

        public virtual User? User { get; init; }
    }
}
