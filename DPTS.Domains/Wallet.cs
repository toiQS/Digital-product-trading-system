using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class Wallet
    {
        [Key]
        [Column("wallet_id")]
        public string WalletId { get; set; } = string.Empty;    

        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Column("balance")]
        public decimal Balance { get; set; } = 0m;

        [Column("currency")]
        [MaxLength(10)]
        public string Currency { get; set; } = "VND";

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; } = null!;
        public virtual ICollection<WalletTransaction> Transactions { get; set; } = new List<WalletTransaction>();
    }

}
