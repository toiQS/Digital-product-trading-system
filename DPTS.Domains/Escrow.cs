using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class Escrow
    {
        [Key]
        [Column("escrow_id")]
        public string EscrowId { get; set; } = string.Empty;

        [Column("order_id")]
        public string OrderId { get; set; } = string.Empty;
        [Column("seller_id")]
        public string SellerId {  get; set; } = string.Empty;

        [Column("amount")]
        public decimal Amount { get; set; }

        [Column("status")]
        public StatusEntity Status { get; set; }

        [Column("created_at")]
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        [Column("updated_at")]
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public virtual Order Order { get; set; } = null!;
        public virtual User User {  get; set; } = null!;  
    }

}
