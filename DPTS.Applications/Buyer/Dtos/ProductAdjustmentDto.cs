namespace DPTS.Applications.Buyer.Dtos
{
    public class ProductAdjustmentDto
    {
        public AdjustmentTypeDto AdjustmentType { get; set; }
        public decimal Price { get; set; } = 0m;
    }
    public enum AdjustmentTypeDto
    {
        Tax,
        Discount,
        PlatformFee,
        FinalPrice 
    }
}
