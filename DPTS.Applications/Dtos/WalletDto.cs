namespace DPTS.Applications.Dtos
{
    public class IndexTradeHistoryDto
    {
        public string TradeIcon { get; set; } = string.Empty;
        public string TradeName { get; set; } = string.Empty;
        public string TradeAt { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public decimal Amount { get; set; }
    }
    public class WalletPersonalDto
    {
        public string WalletId { get; set; } = string.Empty;
        public string UserId { get; set; } = string.Empty;
        public decimal AvaibableBalance { get; set; }
        public string UnitCurrency { get; set; } = string.Empty;
        public List<IndexTradeHistoryDto> IndexTradeHistories { get; set; } = new List<IndexTradeHistoryDto>();
    }
}
