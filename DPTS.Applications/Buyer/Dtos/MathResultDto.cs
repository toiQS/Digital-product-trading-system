using Org.BouncyCastle.X509;

namespace DPTS.Applications.Buyer.Dtos
{
    public class MathResultDto
    {
        public string AdjustmentRuleId { get; set; } = string.Empty;
        public bool IsPercentage {  get; set; }
        public decimal DiscountValue { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal FinalAmount { get; set; }
    }
}
