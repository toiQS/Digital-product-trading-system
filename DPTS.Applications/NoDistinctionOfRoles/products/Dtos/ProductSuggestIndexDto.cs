namespace DPTS.Applications.NoDistinctionOfRoles.products.Dtos
{
    public class ProductSuggestIndexDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public int RatingCount { get; set; }
        public decimal RatingAverage { get; set; }
        public decimal Price { get; set; }
        public string ProductImage { get; set; } = string.Empty;
    }
}
