namespace DPTS.Applications.Dtos.statisticals
{
    public class BaseStatictiscalModel
    {
        public string ImageUrl { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public decimal Quantity { get; set; }
        public decimal Percentage { get; set; }
        public string MoreInfo { get; set; } = string.Empty ;
    }
}
