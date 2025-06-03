using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DPTS.Domains
{
    public class Wallet
    {
        [Key]
        [Column("wallet_id")]
        public string WalletId { get; set; } = string.Empty;    

        [Column("user_id")]
        public string UserId { get; set; } = string.Empty;

        [Column("avaibable_balance")]
        public decimal AvaibableBalance { get; set; } = 0m;

        [Column("currency")]
        [MaxLength(10)]
        public UnitCurrency Currency { get; set; } = UnitCurrency.VND;

        public virtual User User { get; set; } = null!;
        public virtual ICollection<Trade> TradeFroms { get; set; } = new List<Trade>();
        public virtual ICollection<Trade> TradeTos { get; set; } = new List<Trade> { };
    }
    public enum UnitCurrency
    {
        VND,
        USD
    }

}
