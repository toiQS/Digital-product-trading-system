namespace DPTS.Applications.Buyers.payments.Dtos
{
    public class PaymentResultDto
    {
        public string TransactionId { get; set; } = string.Empty;
        public DateTime PaidAt { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
