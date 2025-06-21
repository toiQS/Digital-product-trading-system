namespace DPTS.Applications.NoDistinctionOfRoles.products.Dtos
{
    public class ProductDetailDto
    {
        public string ProductId { get; set; }
        public string Name { get; set; }
        public string SellerId { get; set; }
        public string SellerName { get; set; }
        public string Description { get; set; }
        public decimal Price { get; set; }
        public decimal? OriginalPrice { get; set; }
        public bool IsBestseller { get; set; }
        public int TotalSold { get; set; }
        public double RatingAverage { get; set; }
        public int TotalReviews { get; set; }
        public List<string> Tags { get; set; }
        public List<string> ImageUrls { get; set; }
    }

}
