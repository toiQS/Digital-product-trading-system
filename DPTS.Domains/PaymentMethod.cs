using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public enum PaymentProvider
    {
        Vietcombank,
        MoMo,
        ZaloPay,
        // ...
    }

    public class PaymentMethod
    {
        [Key]
        [Column("payment_method_id")]
        public string PaymentMethodId { get; set; } = string.Empty;
        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;
        [Column("provider")]
        public PaymentProvider Provider { get; set; }
        [Column("masked_account_number")]
        public string? MaskedAccountNumber { get; set; } // "**** 5678" hoặc sdt
        [Column("is_default")]
        public bool IsDefault { get; set; }
        [Column("is_verified")]
        public bool IsVerified { get; set; }
    }

}
