namespace DPTS.Applications.Admin.manage_seller.dtos
{
    public class StoreDto
    {
        public List<StoreIndexDto> StoreIndices { get; set; }
    }
    public class StoreIndexDto
    {
        public string StoreId { get; set; }
        public string StoreName { get; set; }   
        public int CountProduct { get; set; }
        public decimal Revenue { get; set; }  
        public double Rating { get; set; }
        public string Status { get; set; }
        public string StoreImage { get; set; }
    }
}
