using DPTS.Domains;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

public class Log
{
    [Key]
    public string LogId { get; set; } = string.Empty;

    public string? UserId { get; set; }

    [Required]
    [Column(TypeName = "varchar(512)")]
    public string Action { get; set; } = string.Empty;

    public string? TargetType { get; set; }
    public string? TargetId { get; set; }
    [Required]
    public string Description { get; set; } = string.Empty;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public virtual User? User { get; set; }
}
