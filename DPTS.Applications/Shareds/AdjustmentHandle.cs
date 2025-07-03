using DPTS.Applications.Buyer.Dtos;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Shareds
{
    public class AdjustmentHandle : IAdjustmentHandle
    {
        private readonly IAdjustmentRuleRepository _adjustmentRuleRepository;
        private readonly IProductRepository _productRepository;
        private readonly IProductAdjustmentRepository _productAdjustmentRepository;
        private readonly ILogger<AdjustmentHandle> _logger;

        public AdjustmentHandle(
            IAdjustmentRuleRepository adjustmentRuleRepository,
            IProductRepository productRepository,
            IProductAdjustmentRepository productAdjustmentRepository,
            ILogger<AdjustmentHandle> logger)
        {
            _adjustmentRuleRepository = adjustmentRuleRepository;
            _productRepository = productRepository;
            _productAdjustmentRepository = productAdjustmentRepository;
            _logger = logger;
        }

        private ServiceResult<ClassifyAdjustmentDto> ClassifyAdjustment(IEnumerable<AdjustmentRule> adjustmentRules)
        {
            _logger.LogInformation("Classifying adjustment rules...");
            var now = DateTime.Now;
            var taxes = new List<AdjustmentRule>();
            var discounts = new List<AdjustmentRule>();
            var platformFees = new List<AdjustmentRule>();

            foreach (var rule in adjustmentRules)
            {
                if (rule == null || rule.Status != RuleStatus.Active || rule.From > now || rule.To < now || rule.UsageLimit == 0)
                    continue;

                switch (rule.Type)
                {
                    case AdjustmentType.Tax:
                        taxes.Add(rule);
                        break;
                    case AdjustmentType.Discount:
                        discounts.Add(rule);
                        break;
                    case AdjustmentType.PlatformFee:
                        platformFees.Add(rule);
                        break;
                }
            }

            var result = new ClassifyAdjustmentDto
            {
                Taxes = taxes,
                Discounts = discounts,
                PlatformFees = platformFees
            };
            return ServiceResult<ClassifyAdjustmentDto>.Success(result);
        }

        public async Task<ServiceResult<MathResultDto>> HandleDiscountAndPriceForProduct(Product product)
        {
            _logger.LogInformation("Handling discount and price for product...");
            var productAdjustments = await _productAdjustmentRepository.GetRulesByProductIdAsync(product.ProductId);
            var classifyAdjustment = ClassifyAdjustment(productAdjustments);

            if (classifyAdjustment.Status == StatusResult.Errored)
            {
                _logger.LogError("Failed to classify adjustments.");
                return ServiceResult<MathResultDto>.Error("Không thể phân loại các điều chỉnh.");
            }

            var discounts = classifyAdjustment.Data.Discounts;
            var matched = discounts.Where(d => d.Source == AdjustmentSource.Seller && d.Scope == AdjustmentScope.PerProduct && d.TargetLogic == TargetLogic.Auto);

            if (matched.Count() > 1)
                return ServiceResult<MathResultDto>.Error("Có nhiều hơn một giảm giá áp dụng tự động cho sản phẩm.");

            var selected = matched.FirstOrDefault();
            var result = new MathResultDto
            {
                AdjustmentRuleId = selected?.RuleId ?? string.Empty,
                DiscountValue = selected?.Value ?? 0,
                IsPercentage = selected?.IsPercentage ?? false,
                DiscountAmount = selected == null ? 0 : selected.IsPercentage ? product.OriginalPrice * selected.Value : selected.Value,
                FinalAmount = selected == null ? product.OriginalPrice : product.OriginalPrice - (selected.IsPercentage ? product.OriginalPrice * selected.Value : selected.Value)
            };

            return ServiceResult<MathResultDto>.Success(result);
        }

        public async Task<ServiceResult<MathResultDto>> HandleDiscountForOrderAndPayment(string keyCode, Order order)
        {
            _logger.LogInformation("Handling discount for order and voucher...");
            var adjustmentVoucher = await _adjustmentRuleRepository.GetByIdAsync(keyCode);

            if (adjustmentVoucher == null)
            {
                _logger.LogError("Voucher not found.");
                return ServiceResult<MathResultDto>.Error("Không tìm thấy mã giảm giá.");
            }

            if (adjustmentVoucher.TargetLogic != TargetLogic.Voucher)
            {
                _logger.LogError("Invalid voucher logic.");
                return ServiceResult<MathResultDto>.Error("Mã giảm giá không hợp lệ.");
            }

            var voucherAmount = adjustmentVoucher.IsPercentage ? order.TotalAmount * adjustmentVoucher.Value : adjustmentVoucher.Value;
            if (adjustmentVoucher.MaxCap.HasValue && voucherAmount > adjustmentVoucher.MaxCap.Value)
                voucherAmount = adjustmentVoucher.MaxCap.Value;

            var voucherResult = new MathResultDto
            {
                AdjustmentRuleId = adjustmentVoucher.RuleId,
                DiscountValue = adjustmentVoucher.Value,
                IsPercentage = adjustmentVoucher.IsPercentage,
                DiscountAmount = voucherAmount,
                FinalAmount = order.TotalAmount > voucherAmount ? order.TotalAmount - voucherAmount : 0m
            };

            var adjustmentRules = await _adjustmentRuleRepository.GetAllAsync();
            var classifyAdjustment = ClassifyAdjustment(adjustmentRules);

            if (classifyAdjustment.Status == StatusResult.Errored)
            {
                _logger.LogError("Failed to classify adjustments.");
                return ServiceResult<MathResultDto>.Error("Không thể phân loại các điều chỉnh hệ thống.");
            }

            var discounts = classifyAdjustment.Data.Discounts;
            var matched = discounts.Where(d => d.Source == AdjustmentSource.System && d.Scope == AdjustmentScope.PerOrder && d.TargetLogic == TargetLogic.Auto);

            decimal max = 0;
            var bestDiscount = new MathResultDto { FinalAmount = order.TotalAmount };

            foreach (var d in matched)
            {
                var amount = d.IsPercentage ? order.TotalAmount * d.Value : d.Value;
                if (d.MaxCap.HasValue && amount > d.MaxCap.Value)
                    amount = d.MaxCap.Value;
                if (amount < max) continue;

                max = amount;
                bestDiscount = new MathResultDto
                {
                    AdjustmentRuleId = d.RuleId,
                    DiscountValue = d.Value,
                    IsPercentage = d.IsPercentage,
                    DiscountAmount = amount,
                    FinalAmount = order.TotalAmount > amount ? order.TotalAmount - amount : 0m
                };
            }

            return voucherResult.FinalAmount < bestDiscount.FinalAmount
                ? ServiceResult<MathResultDto>.Success(voucherResult)
                : ServiceResult<MathResultDto>.Success(bestDiscount);
        }
    }
}
