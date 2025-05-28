namespace DPTS.Applications.Dtos.orders
{
    public class PreviewOrderDetailModel
    {
        public string  OrderId { get; set; } = string.Empty;

        public List<IndexOrderItemModel> OrderItems { get; set; } = new List<IndexOrderItemModel>();
        public decimal OrderValue { get; set; }
        public float PlatformFee { get; set; }
        public float Tax { get; set; }
        public decimal TotalAmount { get; set; } 

        public int MethodPayment { get; set; }
    }
}
