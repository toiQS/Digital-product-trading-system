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

        [Column("status")]
        public EscrowStatus Status { get; set; } = EscrowStatus.Unknown;

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
        [Column("expired")]
        public DateTime Expired {  get; set; } = DateTime.UtcNow;

        public virtual Order Order { get; set; } = null!;
        public virtual Store Store { get; set; } = null!;
    }

    public enum EscrowStatus
    {
        Unknown = 0,
        WaitingComfirm = 1,
        Comfirmed = 2,
        Done = 3,
        Canceled = 4
    }
}
