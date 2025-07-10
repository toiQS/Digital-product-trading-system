namespace DPTS.Applications.Buyer.Dtos.order
{
    public class DetailOrderDto
    {
        public string StatusOrder { get; set; } = string.Empty;
        public List<DetailItemIndexDto> DetailItemIndexDtos { get; set; } = new List<DetailItemIndexDto>(); 
        public List<PurchasingProcess> PurchasingProcesses { get; set; } = new List<PurchasingProcess> { };
        public DetailInformation DetailInformation { get; set; } = default!;
    }
    public class DetailItemIndexDto
    {
        public string ProductId { get; set; } = string.Empty; 
        public string ProductName {  get; set; } = string.Empty;
        public string ProductImage {  get; set; } = string.Empty;
        public string StoreName {  get; set; } = string.Empty;
    }
    public class PurchasingProcess
    {
        public string NameProcess { get; set; } = string.Empty;
        public string ProessAt {  get; set; } = string.Empty;
        public string Information {  get; set; } = string.Empty;
    }
    public class DetailInformation
    {
        public string PaymentMethodName {  get; set; } = string.Empty;
        public decimal TotalOrder {  get; set; }

    }
}
