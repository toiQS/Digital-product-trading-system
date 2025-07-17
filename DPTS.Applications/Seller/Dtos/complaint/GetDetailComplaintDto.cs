namespace DPTS.Applications.Seller.Dtos.complaint
{
    public class GetDetailComplaintDto
    {
        public string ComplaintId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string ProductId { get; set; } = string.Empty;
        public string EscrowId { get; set; } = string.Empty;
        public string BuyerId { get; set; } = string.Empty;
        public string BuyerName { get; set; } = string.Empty;
        public string MethodContact { get; set; } = string.Empty;   
        public string Description { get; set; } = string.Empty; 
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public List<string> Images { get; set; } = new List<string>();
    }
}
