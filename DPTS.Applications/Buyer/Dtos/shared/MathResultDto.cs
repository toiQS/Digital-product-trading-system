using Org.BouncyCastle.X509;

namespace DPTS.Applications.Buyer.Dtos.shared
{
    public class MathResultDto
    {
        public string AdjustmentRuleId { get; set; } = string.Empty;
        public bool IsPercentage {  get; set; }
        public decimal Value { get; set; }
        public decimal Amount { get; set; }
        public decimal FinalAmount { get; set; }
    }
}
