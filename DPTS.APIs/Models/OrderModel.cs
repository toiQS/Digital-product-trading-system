namespace DPTS.APIs.Models
{
    public class OrderModel
    {
    }
    public class GetOrdersOfSellerModel : PagingModel
    {
        public string SellerId { get; set; } = string.Empty;
    }
}
