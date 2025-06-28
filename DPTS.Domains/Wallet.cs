using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class Wallet
    {
        [Key]
        [Column("wallet_id")]
        public string WalletId { get; set; } = string.Empty;
        [Required]
        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;
        [Column("balance")]
        public decimal Balance { get; set; }

        public List<WalletTransaction> Transactions { get; set; } = new();
    }

}
