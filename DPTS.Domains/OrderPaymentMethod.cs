using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    [Table("order_payment_method")]
    public class OrderPaymentMethod
    {
        private OrderPaymentMethod() { } // For EF

        public OrderPaymentMethod(string orderId, PaymentSourceType sourceType, decimal amount, DateTime paidAt, string? walletId = null, string? paymentMethodId = null)
        {
            OrderPaymentId = Guid.NewGuid().ToString();
            OrderId = orderId;
            SourceType = sourceType;
            WalletId = walletId;
            PaymentMethodId = paymentMethodId;
            Amount = amount;
            PaidAt = paidAt;
        }

        [Key]
        [Column("order_payment_id")]
        public string OrderPaymentId { get; init; }

        [Required]
        [Column("order_id")]
        public string OrderId { get; init; }

        [Column("source_type")]
        public PaymentSourceType SourceType { get; init; }

        [Column("wallet_id")]
        public string? WalletId { get; init; }

        [Column("payment_method_id")]
        public string? PaymentMethodId { get; init; }

        [Column("amount")]
        public decimal Amount { get; init; }

        [Column("paid_at")]
        public DateTime PaidAt { get; init; }

        // Optional navigation (if you want full traceability)
        public virtual Order Order { get; init; } = null!;
        public virtual Wallet? Wallet { get; init; }
        public virtual PaymentMethod? PaymentMethod { get; init; }
    }

    public enum PaymentSourceType
    {
        Wallet,
        PaymentMethod
    }
}
