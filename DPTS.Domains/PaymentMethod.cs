using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public enum PaymentProvider
    {
        Vietcombank,
        MoMo,
        ZaloPay,
        // Có thể thêm: VNPAY, PayPal, Stripe, ...
    }

    [Table("payment_method")]
    public class PaymentMethod
    {
        private PaymentMethod() { }

        public PaymentMethod(
            string userId,
            PaymentProvider provider,
            string? maskedAccountNumber,
            bool isDefault,
            bool isVerified)
        {
            PaymentMethodId = Guid.NewGuid().ToString();
            UserId = userId;
            Provider = provider;
            MaskedAccountNumber = maskedAccountNumber;
            IsDefault = isDefault;
            IsVerified = isVerified;
        }

        [Key]
        [Column("payment_method_id")]
        public string PaymentMethodId { get; init; }

        [Required]
        [Column("user_id")]
        public string UserId { get; init; }

        [Required]
        [Column("provider")]
        public PaymentProvider Provider { get; init; }

        [Column("masked_account_number")]
        public string? MaskedAccountNumber { get; init; }

        [Column("is_default")]
        public bool IsDefault { get; init; }

        [Column("is_verified")]
        public bool IsVerified { get; init; }

        public virtual User User { get; init; } = null!;
    }
}
