using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class Trade
    {
        [Key]
        [Column("trade_id")]
        public string TradeId { get; set; } = string.Empty;

        [Required]
        [Column("trade_name")]
        public string TradeName { get; set; } = string.Empty;

        [Column("amount")]
        public double Amount { get; set; }

        [Required]
        [Column("trade_from_id")]
        public string TradeFromId { get; set; } = string.Empty;

        [Required]
        [Column("trade_to_id")]
        public string TradeToId { get; set; } = string.Empty;

        [Required]
        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        [Column("trade_icon")]
        public string TradeIcon { get; set; } = string.Empty;

        [Column("status")]
        public TradeStatus Status { get; set; } = TradeStatus.Resolving;

        [Column("trade_date")]
        public DateTime TradeDate { get; set; } = DateTime.UtcNow;

        [ForeignKey("TradeFromId")]
        public Wallet? TradeFrom { get; set; }

        [ForeignKey("TradeToId")]
        public Wallet TradeTo { get; set; } = null!;

        [ForeignKey("UserId")]
        public User User { get; set; } = null!;
    }

    public enum TradeStatus
    {
        Resolving = 0,
        Done = 1,
        Errored = 2,
    }
}
