namespace DPTS.Applications.Dtos.orders
{
    public class IndexOrderItemModel
    {
        public string OrderItemId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string SellerName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
    }
}
