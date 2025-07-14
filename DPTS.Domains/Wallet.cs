using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    [Table("wallet")]
    public class Wallet
    {
        private Wallet() { } // For EF

        public Wallet(string userId)
        {
            WalletId = Guid.NewGuid().ToString();
            UserId = userId;
            Balance = 0m;
            LockedBalance = 0m;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
        }

        [Key]
        [Column("wallet_id")]
        public string WalletId { get; init; }

        [Required]
        [Column("user_id")]
        public string UserId { get; init; }

        [Column("balance")]
        public decimal Balance { get; set; }

        [Column("locked_balance")]
        public decimal LockedBalance { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; init; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; }

        public virtual ICollection<WalletTransaction> Transactions { get; init; } = new List<WalletTransaction>();
        public virtual User User { get; init; } = null!;
    }

}
