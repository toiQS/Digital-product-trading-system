namespace DPTS.Applications.Buyer.Dtos.order
{
    public class CheckoutSummaryDto
    {
        public string DiscountId { get; set; } = string.Empty;
        public decimal Value {  get; set; }
        public decimal FinalAmount { get; set; }
    }
}
