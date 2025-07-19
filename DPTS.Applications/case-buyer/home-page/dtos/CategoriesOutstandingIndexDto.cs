namespace DPTS.Applications.case_buyer.homepage.dtos
{
    public record CategoriesOutstandingIndexDto
    {
        public string CategoryName { get; init; }
        public string CategoryId { get; init; } 
        public string CategoryImage { get; init; }
        public int CountProductsAvailable { get; init; }
    }
}
