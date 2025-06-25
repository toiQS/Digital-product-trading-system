namespace DPTS.Applications.Buyers.payments.Dtos
{
    public class CheckoutItemDto
    {
        public string ProductId { get; set; } = string.Empty;
        public string ProductName { get; set; } = string.Empty;
        public string StoreName { get; set; } = string.Empty;
        public string ProductImage { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public decimal Amount { get; set; }
    }
}
