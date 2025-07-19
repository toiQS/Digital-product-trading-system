namespace DPTS.Applications.case_buyer.homepage.dtos
{
    public record TopSateProductIndexDto
    {
        public string ProductId { get; init; }
        public string ProductName { get; init; }
        public string ProductImage { get; init; }
        public decimal Price { get; init; }
        public int CountRating { get; init; }
        public double AverageRating { get; init; }
    }
}
