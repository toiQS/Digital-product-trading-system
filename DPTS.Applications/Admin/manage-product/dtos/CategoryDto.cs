namespace DPTS.Applications.Admin.manage_product.dtos
{
    public class CategoryDto
    {
        public List<CategoryIndexDto> CategoryIndexDtos { get; set; }
    }
    public class CategoryIndexDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
