using DPTS.Domains;

namespace DPTS.Applications.Seller.conplaints.Dtos
{
    public class ComplaintListItemDto
    {
        public string ComplaintId { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public string ContentPreview { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public string OrderCode { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
    }

}
