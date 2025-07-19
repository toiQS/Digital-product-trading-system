using DPTS.Domains;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPTS.Applications.shareds.maths
{
    public class MathForProduct
    {
        public static MathResult CalculatePrice(Product product, AdjustmentRule adjustmentRule)
        {
            var result = new MathResult
            {
                OriginalPrice = product.OriginalPrice,
                DiscountValue = adjustmentRule.Value,
                DiscountAmount = adjustmentRule.IsPercentage
                    ? product.OriginalPrice * adjustmentRule.Value / 100
                    : adjustmentRule.Value,
                
            };
            result.FinalPrice = product.OriginalPrice - result.DiscountAmount;
            return result;
        }
    }
    public record MathResult
    {
        public decimal FinalPrice { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal OriginalPrice { get; set; }
        public bool IsPercentage { get; set; } 
        public decimal DiscountValue { get; set; }
    }
}
