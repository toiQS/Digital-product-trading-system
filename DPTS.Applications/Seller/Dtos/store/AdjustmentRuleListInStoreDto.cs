using MailKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPTS.Applications.Seller.Dtos.store
{
    public class AdjustmentRuleListInStoreDto
    {
        public int TotalCount { get; set; } = 0;
        public List<AdjustmentRuleIndexListInStoreDto> AdjustmentRules { get; set; } = new List<AdjustmentRuleIndexListInStoreDto>();
    }
    public class AdjustmentRuleIndexListInStoreDto
    {
        public string RuleId { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public decimal? MinOrderAmount { get; set; }
        public bool IsPercentage { get; set; } = true;
        public decimal Value { get; set; } = 0.0m;
        public decimal? MaxCap { get; set; }
        public int? UsageLimit { get; set; }
        public int? PerUserLimit { get; set; }
        public string? VoucherCode { get; set; } = null;
        public string Status { get; set; }
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
    }
}
