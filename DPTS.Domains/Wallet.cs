using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    [Table("wallet")]
    public class Wallet
    {
        public Wallet(string userId)
        {
            WalletId = Guid.NewGuid().ToString();
            UserId = userId;
            Balance = 0m;
            Transactions = new List<WalletTransaction>();
        }

        [Key]
        [Column("wallet_id")]
        public string WalletId { get; init; }

        [Required]
        [Column("user_id")]
        public string UserId { get; init; }

        [Column("balance")]
        public decimal Balance { get; init; }

        public virtual ICollection<WalletTransaction> Transactions { get; init; } = new List<WalletTransaction>();
        private Wallet() { }
    }
}
