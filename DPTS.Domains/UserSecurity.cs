using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    [Table("user_security")]
    public class UserSecurity
    {
        private UserSecurity() { } // For EF

        public UserSecurity(
            string userId,
            string passwordHash,
            bool twoFactorEnabled,
            string? twoFactorSecret,
            bool emailVerified,
            int failedLoginAttempts,
            DateTime? lockoutUntil)
        {
            UserId = userId;
            PasswordHash = passwordHash;
            TwoFactorEnabled = twoFactorEnabled;
            TwoFactorSecret = twoFactorSecret;
            EmailVerified = emailVerified;
            FailedLoginAttempts = failedLoginAttempts;
            LockoutUntil = lockoutUntil;
            UpdatedAt = DateTime.UtcNow;
        }

        [Key]
        [ForeignKey("User")]
        [Column("user_id")]
        public string UserId { get; init; }

        [Required]
        [Column("password_hash")]
        public string PasswordHash { get; init; }

        [Column("two_factor_enabled")]
        public bool TwoFactorEnabled { get; init; }

        [Column("two_factor_secret")]
        public string? TwoFactorSecret { get; init; }

        [Column("email_verified")]
        public bool EmailVerified { get; init; }

        [Column("failed_login_attempts")]
        public int FailedLoginAttempts { get; init; }

        [Column("lockout_until")]
        public DateTime? LockoutUntil { get; init; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        public virtual User User { get; init; } = null!;
    }
}
