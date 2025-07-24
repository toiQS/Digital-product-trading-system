namespace DPTS.Applications.Admin.manage_product.dtos
{
    public class ProductDto
    {
        public List<ProductIndexDto> ProductIndexDtos { get; set; }
        public int ProductCount { get; set; }
    }
    public class ProductIndexDto
    {
        public string ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductImage { get; set; }
        public string StoreId{ get; set; }
        public string StoreName { get; set; }
        public string CategoryId { get; set; }
        public string CategoryName { get; set; }
        public decimal Price { get; set; }
        public string Status { get; set; }
    }
}
