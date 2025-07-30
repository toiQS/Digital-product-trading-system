namespace DPTS.Applications.Admin.manage_revenue.dtos
{
    public class TopStoreDto
    {
        public List<TopStoreIndexDto> Indexs { get; set; }  = new List<TopStoreIndexDto>();
    }
    public class TopStoreIndexDto
    {
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreImage { get; set; }
        public int CountProduct {  get; set; }
        public string Revenue { get; set; }
        public double AverageRating { get; set; }
    }
}
