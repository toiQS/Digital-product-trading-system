namespace DPTS.Applications.Buyers.payments.Dtos
{
    public class CheckoutSummaryDto
    {
        public decimal Subtotal { get; set; }
        public decimal PlatformFee { get; set; }
        public decimal Tax { get; set; }
        public decimal Total { get; set; }
    }
}
