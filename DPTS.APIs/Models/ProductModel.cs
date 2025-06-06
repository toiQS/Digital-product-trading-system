namespace DPTS.APIs.Models
{
    public class ProductModel
    {

    }
    public class GetProductOfSellerModel : PagingModel
    {
        public string SellerId { get; set; } = string.Empty;
    }
    public class GetDetailProductModel
    {
        public string ProductId { get; set; } = string.Empty;
    }
    public class GetProductsWithManyOptionsModel : PagingModel
    {
        public string Text { get; set; } = string.Empty;
        public string CategoryId { get; set; } = string.Empty;
        public int Rating { get; set; } = 0;
    }
}
