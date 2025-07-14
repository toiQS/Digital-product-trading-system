using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    [Table("escrow")]
    public class Escrow
    {
        private Escrow() { } // For EF

        public Escrow(string orderId, string storeId, decimal amount, decimal platformFeeRate, decimal taxRate)
        {
            EscrowId = Guid.NewGuid().ToString();
            OrderId = orderId;
            StoreId = storeId;
            Amount = amount;

            PlatformFeeRate = platformFeeRate;
            PlatformFeeAmount = Math.Round(amount * platformFeeRate, 2);

            TaxRate = taxRate;
            TaxAmount = Math.Round(amount * taxRate, 2);

            ActualAmount = amount - PlatformFeeAmount - TaxAmount;

            Status = EscrowStatus.Pending;
            CreatedAt = DateTime.UtcNow;
            UpdatedAt = DateTime.UtcNow;
            Expired = CreatedAt.AddDays(30); // hết hạn sau 7 ngày
        }

        [Key]
        [Column("escrow_id")]
        public string EscrowId { get; init; }

        [Required]
        [Column("order_id")]
        public string OrderId { get; init; }

        [Required]
        [Column("store_id")]
        public string StoreId { get; init; }

        [Column("amount")]
        public decimal Amount { get; init; }

        [Column("platform_fee_rate")]
        public decimal PlatformFeeRate { get; init; }

        [Column("platform_fee_amount")]
        public decimal PlatformFeeAmount { get; init; }

        [Column("tax_rate")]
        public decimal TaxRate { get; init; }

        [Column("tax_amount")]
        public decimal TaxAmount { get; init; }

        [Column("actual_amount")]
        public decimal ActualAmount { get; init; }

        [Column("released_at")]
        public DateTime? ReleasedAt { get; private set; }

        [Column("released_by")]
        public string? ReleasedBy { get; private set; }

        [Column("status")]
        public EscrowStatus Status { get; private set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; init; }

        [Column("updated_at")]
        public DateTime UpdatedAt { get; private set; }

        [Column("expired")]
        public DateTime Expired { get; init; }

        public virtual Order Order { get; init; } = null!;
        public virtual Store Store { get; init; } = null!;

        // Domain logic
        public void MarkAvailable()
        {
            Status = EscrowStatus.Avalible;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkComplaint()
        {
            Status = EscrowStatus.BeenComplainting;
            UpdatedAt = DateTime.UtcNow;
        }

        public void MarkReleased(string releasedBy)
        {
            Status = EscrowStatus.Avalible; // hoặc thêm trạng thái Released nếu có enum mới
            ReleasedBy = releasedBy;
            ReleasedAt = DateTime.UtcNow;
            UpdatedAt = ReleasedAt.Value;
        }

        public void MarkError()
        {
            Status = EscrowStatus.Error;
            UpdatedAt = DateTime.UtcNow;
        }
    }

    public enum EscrowStatus
    {
        Unknown = 0,
        Pending,             // Đang giữ tiền
        Avalible,            // Có thể release
        BeenComplainting,    // Đang khiếu nại
        Error                // Lỗi hệ thống
    }
}
