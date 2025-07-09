namespace DPTS.Applications.Buyer.Dtos.wallet
{
    public class WalletDto
    {
        public string WalletId { get; set; } = string.Empty;
        public decimal Balance { get; set; }
        public List<TradeDto> Trades { get; set; } = new List<TradeDto>();
        public List<PaymentMethodDto> PaymentMethods { get; set; } = new List<PaymentMethodDto>();
    }
}
