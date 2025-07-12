namespace DPTS.Applications.Seller.orders.Dtos
{
    public class OrderListItemDto
    {
        public string EscrowId { get; set; } = string.Empty;
        public string BuyerName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public string TimeBuyAt { get; set; } = string.Empty;
        public string DateBuyAt { get; set; } = string.Empty;
    }
}
