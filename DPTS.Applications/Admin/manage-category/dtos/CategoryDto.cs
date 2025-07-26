namespace DPTS.Applications.Admin.manage_category.dtos
{
    public class CategoryDto
    {
        public int Count { get; set; }
        public List<CategoryIndexDto> IndexDtos { get; set; }   
    }
    public class CategoryIndexDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string ImageUrl { get; set; }
        public string Information { get; set; }
        public string Description { get; set; }
        public int DisplayOrder { get; set; }
    }
}
