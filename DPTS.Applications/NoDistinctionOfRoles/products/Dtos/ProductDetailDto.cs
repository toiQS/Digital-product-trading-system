using System.Text;

namespace DPTS.Applications.NoDistinctionOfRoles.products.Dtos
{
    public class ProductDetailDto
    {
        // Header
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public double Rating { get; set; }
        public int CountReviews { get; set; }
        public string StoreImage { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal OriginalPrice { get; set; }
        public int Discount { get; set; }
        public string Summary { get; set; } = string.Empty;

        public List<string> ProductImage { get; set; } = new();

        // Body
        public string Description { get; set; } = string.Empty;
        public decimal Vote1 { get; set; }
        public decimal Vote2 { get; set; }
        public decimal Vote3 { get; set; }
        public decimal Vote4 { get; set; }
        public decimal Vote5 { get; set; }
        public List<ProductReviewIndexDto> ProductReviewIndex { get; set; } = new List<ProductReviewIndexDto> { };
        public List<ProductSuggestIndexDto> ProductSuggestIndex { get; set; } = new List<ProductSuggestIndexDto> { };
    }
}
