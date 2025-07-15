namespace DPTS.Applications.Buyer.Dtos.wallet
{
    public class TradeDto
    {
        public string Description { get; set; } = string.Empty; 
        public string TradeId { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string TradeAt { get; set; } = string.Empty;
    }
}
