using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public enum PaymentSourceType
    {
        Wallet,
        PaymentMethod
    }

    public class OrderPayment
    {
        [Key]
        [Column("order_payment_id")]
        public string OrderPaymentId { get; set; } = string.Empty;
        [Column("order_id")]
        public string OrderId { get; set; } = string.Empty;
        [Column("source_type")]
        public PaymentSourceType SourceType { get; set; }
        [Column("wallet_id")]
        public string? WalletId { get; set; } // nếu chọn ví
        [Column("payment_method_id")]
        public string? PaymentMethodId { get; set; } // nếu chọn MoMo, ngân hàng
        [Column("amount")]
        public decimal Amount { get; set; }
        [Column("paid_at")]
        public DateTime PaidAt { get; set; }
    }

}
