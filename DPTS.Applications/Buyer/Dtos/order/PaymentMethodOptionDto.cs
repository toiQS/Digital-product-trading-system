namespace DPTS.Applications.Buyer.Dtos.order
{
    public class PaymentMethodOptionDto
    {
        public string PaymentMethodId { get; set; } = string.Empty;
        public string MethodCode { get; set; } = string.Empty; // "wallet", "momo", "bank"
        public string MethodName { get; set; } = string.Empty; // "Ví sàn", "MoMo", etc
        public string Description { get; set; } = string.Empty;
        public bool IsRecommended { get; set; } = false;
        public decimal? AvailableBalance { get; set; } // cho ví
    }
}
