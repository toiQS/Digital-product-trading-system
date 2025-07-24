namespace DPTS.Applications.Admin.manage_product.dtos
{
    public class OverviewDto
    {
        public List<OverviewIndexDto> IndexDtos { get; set; }
    }
    public class OverviewIndexDto
    {
        public string Name{ get; set; }
        public decimal Value{ get; set; }
    }
}
