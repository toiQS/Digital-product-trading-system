namespace DPTS.Applications.Dtos.products
{
    public class ProductIndexModel
    {
        public string Image { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Category {  get; set; } = string.Empty;
        public int QuantitySelled { get; set; } 
        public decimal Price { get; set; }
        public string Status {  get; set; } = string.Empty;
        public float Rating { get; set; } = 0.0f;
    }
}
