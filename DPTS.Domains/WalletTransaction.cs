using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class WalletTransaction
    {
        [Key]
        [Column("transaction_id")]
        public string TransactionId { get; set; } = string.Empty;

        [Column("wallet_id")]
        public string WalletId { get; set; } = string.Empty;

        [Required]
        [Column("type")]
        public string Type { get; set; } = string.Empty;

        [Required]
        [Column("amount")]
        public decimal Amount { get; set; }

        [Required]
        [Column("status")]
        public string Status { get; set; } = string.Empty;

        [Column("payment_method")]
        public string? PaymentMethod { get; set; }

        [Column("transaction_reference")]
        public string? TransactionReference { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual Wallet Wallet { get; set; } = null!;
    }

}
