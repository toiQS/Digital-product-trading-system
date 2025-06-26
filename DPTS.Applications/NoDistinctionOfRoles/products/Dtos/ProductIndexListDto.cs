namespace DPTS.Applications.NoDistinctionOfRoles.products.Dtos
{
    public class ProductIndexListDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public double RatingOverallAverage { get; set; }
        //public int RatingOverallCount { get; set; }
        public int QuantitySold { get; set; }

        public string ProductImage {  get; set; } = string.Empty;
        public decimal Price {  get; set; }
        public string CategoryName {  get; set; } = string.Empty;
    }
}
