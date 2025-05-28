using DPTS.Applications.Dtos.images;
using DPTS.Applications.Dtos.reviews;

namespace DPTS.Applications.Dtos.products
{
    public class ProductDetailModel
    {
        public string ProductId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string CategoryName { get; set; } = string.Empty;       
        public string Status { get; set; } = string.Empty;
        public float Rating { get; set; } = 0.0f;
        public List<IndexImageModel> Images { get; set; } = new List<IndexImageModel>();
        public List<IndexProductReviewModel> Reviews { get; set; } = new List<IndexProductReviewModel>();

    }
}
