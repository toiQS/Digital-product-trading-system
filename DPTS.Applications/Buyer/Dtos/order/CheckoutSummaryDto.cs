namespace DPTS.Applications.Buyer.Dtos.order
{
    public class CheckoutSummaryDto
    {
        public string DiscountId { get; set; } = string.Empty;
        public decimal DiscountValue {  get; set; }
        public decimal FinalAmount { get; set; }
    }
}
