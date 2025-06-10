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
        public string TradeName { get; set;} = string.Empty;
        [Column("amount")]
        public double Amount { get; set; }
        [Column("trade_from_id")]
        public string TradeFromId { get; set; } = string.Empty;
        [Column("trade_to_id")]
        public string TradeToId { get; set;} = string.Empty;
        [Column("trade_icon")]
        [Required]
        public string TradeIcon { get; set; } = string.Empty;
        [Column("status")]
        public TradeStatus Status { get; set; }
        public DateTime TradeDate { get; set;}
        public Wallet? TrandeFrom { get; set; }
        public Wallet TrandeTo { get; set; } = null!;
        public User User { get; set; } = null!;
    }
    public enum TradeStatus
    { 
        Resolving,
        Done,
        Errored,
    }
}
