namespace DPTS.Applications.NoDistinctionOfRoles.homePages.Dtos
{
    public class FeaturedCategoryDto
    {
        public string CategoryId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public string CategoryIcon { get; set; } = string.Empty;
    }

}
