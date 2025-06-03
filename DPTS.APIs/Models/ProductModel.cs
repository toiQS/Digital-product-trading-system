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
    public class GetProductsByCategoryAndRatingModel : PagingModel
    {
        public string CategoryId { get; set; } = string.Empty;
        public int Rating { get; set; }
    }
    public class GetProductCanBeLike : PagingModel
    {
        public string CategoryId { get; set; } = string.Empty;
    }
}
