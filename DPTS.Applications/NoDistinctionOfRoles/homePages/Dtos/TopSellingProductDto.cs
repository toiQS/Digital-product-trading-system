namespace DPTS.Applications.NoDistinctionOfRoles.homePages.Dtos
{
    public class TopSellingProductDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public double RatingOverall { get; set; }
        public int SoldQuantity { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }

}
