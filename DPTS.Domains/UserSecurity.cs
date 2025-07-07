using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class UserSecurity
    {
        [Key]
        [ForeignKey("User")]
        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Column("password_hash")]
        public string PasswordHash { get; set; } = string.Empty;

        [Column("two_factor_enabled")]
        public bool TwoFactorEnabled { get; set; } = false;

        [Column("two_factor_secret")]
        public string? TwoFactorSecret { get; set; }

        [Column("email_verified")]
        public bool EmailVerified { get; set; } = false;

        [Column("failed_login_attempts")]
        public int FailedLoginAttempts { get; set; } = 0;

        [Column("lockout_until")]
        public DateTime? LockoutUntil { get; set; }

        public virtual User User { get; set; } = null!;
    }
}
