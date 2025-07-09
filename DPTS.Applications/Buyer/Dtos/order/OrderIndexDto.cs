namespace DPTS.Applications.Buyer.Dtos.order
{
    public class OrderIndexDto
    {
        public string OrderId { get; set; } = string.Empty;
        public string BuyAt {  get; set; } = string.Empty;
        public string Status {  get; set; } = string.Empty;
        public decimal Amount {  get; set; }
    }
}
