using System.Text;

namespace DPTS.Applications.Dtos
{
    public class OrderIndexDto
    {
        public string OrderId { get; set; } = string.Empty;
        public string BuyerName { get; set; } = string.Empty;
        public string ProductionName { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
    public class OrderItemDto
    {
        public string OrderItemId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
