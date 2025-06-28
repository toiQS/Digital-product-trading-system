using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class User
    {
        [Key]
        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        [Column("username")]
        public string Username { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        [Column(" email")]
        public string Email { get; set; } = string.Empty;

        [Column("role_id")]
        public string RoleId { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("is_active")]
        public bool IsActive { get; set; }

        // Navigation
        public virtual Role Role { get; set; } = null!;
        public virtual UserProfile Profile { get; set; } = null!;
        public virtual UserSecurity Security { get; set; } = null!;

        // Optional relations
        public virtual Wallet? Wallet { get; set; }
        public virtual Store? Store { get; set; }

        // Collections
        public virtual ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();
    }
}
