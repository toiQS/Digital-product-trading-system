namespace DPTS.Applications.Admin.manage_seller.dtos
{
    public class OverviewDto
    {
        public List<OverviewIndexDto> Indexs { get; set; }
    }
    public class OverviewIndexDto
    {
        public string Name { get; set; }
        public int Value { get; set; }
    }
}
