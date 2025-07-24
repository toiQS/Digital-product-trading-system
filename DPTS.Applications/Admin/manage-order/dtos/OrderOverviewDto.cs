namespace DPTS.Applications.Admin.manage_order.dtos
{
    public   class OrderOverviewDto
    {
        public List<OrderOverviewIndexDto> OrderOverviewIndexDtos { get; set; }
    }
    public class OrderOverviewIndexDto
    {
        public string Name { get; set; }
        public decimal Value { get; set; }
        public string Information { get; set; }
    }
}
