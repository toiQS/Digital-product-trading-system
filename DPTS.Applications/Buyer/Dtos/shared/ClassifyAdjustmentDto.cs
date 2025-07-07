using DPTS.Domains;

namespace DPTS.Applications.Buyer.Dtos.shared
{
    public class ClassifyAdjustmentDto
    {
        public IEnumerable<AdjustmentRule> Taxes { get; set; } = new List<AdjustmentRule>();
        public IEnumerable<AdjustmentRule> Discounts { get; set; } = new List<AdjustmentRule>();
        public IEnumerable<AdjustmentRule> PlatformFees { get; set; } = new List<AdjustmentRule> { };
    }
}
