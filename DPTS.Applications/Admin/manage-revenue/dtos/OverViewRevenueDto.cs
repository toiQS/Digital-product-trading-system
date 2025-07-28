namespace DPTS.Applications.Admin.manage_revenue.dtos
{
    public class OverViewRevenueDto
    {
        public List<OverviewRevenueIndexDto> OverviewRevenueIndexDtos { get; set; }
    }
    public class OverviewRevenueIndexDto
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string Information { get; set; }
    }
}
