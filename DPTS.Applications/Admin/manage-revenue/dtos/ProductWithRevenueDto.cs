namespace DPTS.Applications.Admin.manage_revenue.dtos
{
    public class ProductWithRevenueDto
    {
        public List<ProductWithRevenueIndexDto> Indexs { get; set; }
    }
    public class ProductWithRevenueIndexDto
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Sold { get; set; }
        public string Revenue { get; set; }
    }
}
