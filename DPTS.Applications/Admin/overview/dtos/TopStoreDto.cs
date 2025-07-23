namespace DPTS.Applications.Admin.overview.dtos
{
    public class TopStoreDto
    {
        public List<TopStoreIndexDto> Indexs { get; set; }
    }
    public class TopStoreIndexDto
    {
        public string StoreId { get; set; }
        public string StoreName { get; set; }
        public string StoreImage { get; set; }
        public int CountProduct {  get; set; }
        public decimal Revenue { get; set; }
        public double AverageRating { get; set; }
    }
}
