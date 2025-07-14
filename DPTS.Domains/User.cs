using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    [Table("user")]
    public class User
    {
        private User() { } // For EF

        public User(string username, string email, string roleId)
        {
            UserId = Guid.NewGuid().ToString();
            Username = username;
            Email = email;
            RoleId = roleId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            IsActive = true;
            PaymentMethods = new List<PaymentMethod>();
        }

        [Key]
        [Column("user_id")]
        public string UserId { get; init; }

        [Required]
        [MaxLength(50)]
        [Column("username")]
        public string Username { get; init; }

        [Required]
        [MaxLength(100)]
        [Column("email")]
        public string Email { get; init; }

        [Required]
        [Column("role_id")]
        public string RoleId { get; init; }

        [Column("created_at")]
        public DateTime CreatedAt { get; init; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        [Column("is_active")]
        public bool IsActive { get; set; }

        // Navigation
        public virtual UserRole UserRole { get; init; } = null!;
        public virtual UserProfile Profile { get; init; } = null!;
        public virtual UserSecurity Security { get; init; } = null!;
        public virtual Wallet? Wallet { get; init; }
        public virtual Store? Store { get; init; }
        public virtual ICollection<PaymentMethod> PaymentMethods { get; init; } = new List<PaymentMethod>();
    }
}
