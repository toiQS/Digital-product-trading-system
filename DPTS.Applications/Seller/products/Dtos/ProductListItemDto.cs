namespace DPTS.Applications.Seller.products.Dtos
{
    public class ProductListItemDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string Category { get; set; } = string.Empty; // e.g. Ebook, Khóa học
        public decimal Price { get; set; }
        public int SoldCount { get; set; }
        public string Status { get; set; } = string.Empty; // "Hoạt động", "Đang chờ", "Bị chặn"
        public string ProductImage { get; set; } = string.Empty;
    }
}
