using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public enum TransactionType
    {
        Deposit,
        Withdraw,
        Purchase,
        Refund
    }
    public enum WalletTransactionStatus
    {
        Pending,     // Đang xử lý
        Completed,   // Hoàn thành
        Failed       // Thất bại
    }
    public class WalletTransaction
    {
        [Key]
        [Column("wallet_transaction_id")]
        public string TransactionId { get; set; } = string.Empty;
        [Column("wallet_id")]
        public string WalletId { get; set; } = string.Empty;
        [Column("type")]
        public TransactionType Type { get; set; }
        [Column("amount")]
        public decimal Amount { get; set; }
        [Column("timestamp")]
        public DateTime Timestamp { get; set; }
        [Column("description")]
        public string? Description { get; set; }
        [Column("status")]
        public WalletTransactionStatus Status { get; set; }

        public PaymentMethod? LinkedPaymentMethod { get; set; } // nếu nạp/rút
    }

}
