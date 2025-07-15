using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class Escrow
    {
        [Key]
        [Column("escrow_id")]
        public string EscrowId { get; set; } = string.Empty;

        [Required]
        [Column("order_id")]
        public string OrderId { get; set; } = string.Empty;

        [Required]
        [Column("store_id")]
        public string StoreId { get; set; } = string.Empty;

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("platform_fee_rate")]
        public decimal PlatformFeeRate { get; set; }

        [Column("platform_fee_amount")]
        public decimal PlatformFeeAmount { get; set; }

        [Column("tax_rate")]
        public decimal TaxRate { get; set; }

        [Column("tax_amount")]
        public decimal TaxAmount { get; set; }
        [Column("actual_amount")]
        public decimal ActualAmount { get; set; } // giá trị thực thế mà cửa hàng nhận được
        [Column("released_at")]
        public DateTime? ReleasedAt { get; set; }

        [Column("released_by")]
        public string? ReleasedBy { get; set; }  // e.g. "System", "Admin:duyanh", etc.


        [Column("status")]
        public EscrowStatus Status { get; set; } = EscrowStatus.Unknown;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        [Column("expired")]
        public DateTime Expired { get; set; } = DateTime.UtcNow;

        public virtual Order Order { get; set; } = null!;
        public virtual Store Store { get; set; } = null!;
    }

    public enum EscrowStatus
    {
        Unknown,           // Không rõ
        Pending,           // Đang xử lý
        WaitingConfirm,    // Chờ xác nhận
        BuyerConfirmed,    // Đã xác nhận
        Done,              // Hoàn tất
        Complaint,         // Khiếu nại
        Canceled,          // Đã huỷ
        Failed             // Lỗi
    }

}
