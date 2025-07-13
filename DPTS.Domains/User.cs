using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    [Table("user")]
    public class User
    {
        public User(string username, string email, string roleId)
        {
            UserId = Guid.NewGuid().ToString();
            Username = username;
            Email = email;
            RoleId = roleId;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        [Key, Column("user_id")]
        public string UserId { get; init; } = Guid.NewGuid().ToString();

        [Required, MaxLength(50), Column("username")]
        public string Username { get; set; } = string.Empty;

        [Required, MaxLength(100), Column("email")]
        public string Email { get; set; } = string.Empty;

        [Column("role_id")]
        public string RoleId { get; set; } = string.Empty;

        [Column("created_at")]
        public DateTime CreatedAt { get; init; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("is_active")]
        public bool IsActive { get; set; } = true;

        public virtual UserRole UserRole { get; set; } = null!;
        public virtual UserProfile Profile { get; set; } = null!;
        public virtual UserSecurity Security { get; set; } = null!;
        public virtual Wallet? Wallet { get; set; }
        public virtual Store? Store { get; set; }
        public virtual ICollection<PaymentMethod> PaymentMethods { get; set; } = new List<PaymentMethod>();
    }

}
