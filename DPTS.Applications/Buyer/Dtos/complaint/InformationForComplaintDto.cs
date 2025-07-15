namespace DPTS.Applications.Buyer.Dtos.complaint
{
    public class InformationForComplaintDto
    {
        public List<OrderItemPurchased> OrderItemPurchaseds { get; set; }
        public List<RecentlyComplaintDto> RecentlyComplaint { get; set; }
    }
    public class OrderItemPurchased
    {
        public string OrderItemId {  get; set; } = string.Empty;
        public string ProductName {  get; set; } = string.Empty;
        public DateTime BuyAt { get; set; }
    }
    public class RecentlyComplaintDto
    {
        public string EscrowId { get; set; } = string.Empty;
        public DateTime ComplaintAt { get; set; }
        public string Status { get; set; } = string.Empty;
    }
}
    