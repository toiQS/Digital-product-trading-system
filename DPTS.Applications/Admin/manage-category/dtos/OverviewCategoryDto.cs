namespace DPTS.Applications.Admin.manage_category.dtos
{
    public class OverviewCategoryDto
    {
        public List<OverviewCategoryIndexDto> IndexDtos { get; set; }
    }
    public class OverviewCategoryIndexDto
    {
        public string Name { get; set; }
        public decimal Value { get; set; }
    }
}
