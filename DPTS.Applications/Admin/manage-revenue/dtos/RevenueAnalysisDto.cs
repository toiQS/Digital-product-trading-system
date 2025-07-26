namespace DPTS.Applications.Admin.manage_revenue.dtos
{
    public class RevenueAnalysisDto
    {
        public List<RevenueAnalysisIndexDto> Indexs { get; set; }
    }
    public class RevenueAnalysisIndexDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
    }
}
