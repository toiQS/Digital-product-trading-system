namespace DPTS.Applications.Buyer.Dtos.order
{
    public class CheckoutDto
    {
        public List<CheckoutItemDto> Items { get; set; } = new();
        public CheckoutSummaryDto Summary { get; set; } = new();
        public List<PaymentMethodOptionDto> PaymentMethods { get; set; } = new();
    }
}
