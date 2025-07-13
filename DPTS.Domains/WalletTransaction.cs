using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public enum TransactionType
    {
        Unknown,
        Deposit,
        Withdraw,
        Purchase,
        Refund
    }

    public enum WalletTransactionStatus
    {
        Unknown,
        Pending,
        Completed,
        Failed
    }

    [Table("wallet_transaction")]
    public class WalletTransaction
    {
        public WalletTransaction(
            string walletId,
            TransactionType type,
            decimal amount,
            string? description,
            WalletTransactionStatus status)
        {
            TransactionId = Guid.NewGuid().ToString();
            WalletId = walletId;
            Type = type;
            Amount = amount;
            Timestamp = DateTime.UtcNow;
            Description = description;
            Status = status;
        }

        [Key]
        [Column("wallet_transaction_id")]
        public string TransactionId { get; init; }

        [Column("wallet_id")]
        public string WalletId { get; init; }

        [Column("type")]
        public TransactionType Type { get; init; }

        [Column("amount")]
        public decimal Amount { get; init; }

        [Column("timestamp")]
        public DateTime Timestamp { get; init; }

        [Column("description")]
        public string? Description { get; init; }

        [Column("status")]
        public WalletTransactionStatus Status { get; init; }

        public virtual PaymentMethod? LinkedPaymentMethod { get; init; }
    }
}
