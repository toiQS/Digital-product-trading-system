using DPTS.Applications.Buyer.Dtos.shared;
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
                if (rule == null || rule.Status != RuleStatus.Active || rule.From > now || rule.To < now)
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
                Value = selected?.Value ?? 0,
                IsPercentage = selected?.IsPercentage ?? false,
                Amount = selected == null ? 0 : selected.IsPercentage ? product.OriginalPrice * selected.Value : selected.Value,
                FinalAmount = selected == null ? product.OriginalPrice : product.OriginalPrice - (selected.IsPercentage ? product.OriginalPrice * selected.Value : selected.Value)
            };

            return ServiceResult<MathResultDto>.Success(result);
        }

        public async Task<ServiceResult<MathResultDto>> HandleDiscountForOrderAndPayment(string keyCode = null!, Order order = default!)
        {
            _logger.LogInformation("Handling discount for order and voucher...");
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
                    Value = d.Value,
                    IsPercentage = d.IsPercentage,
                    Amount = amount,
                    FinalAmount = order.TotalAmount > amount ? order.TotalAmount - amount : 0m
                };
            }

            if (keyCode != null)
            {
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
                    Value = adjustmentVoucher.Value,
                    IsPercentage = adjustmentVoucher.IsPercentage,
                    Amount = voucherAmount,
                    FinalAmount = order.TotalAmount > voucherAmount ? order.TotalAmount - voucherAmount : 0m
                };
                return voucherResult.FinalAmount < bestDiscount.FinalAmount
                    ? ServiceResult<MathResultDto>.Success(voucherResult)
                    : ServiceResult<MathResultDto>.Success(bestDiscount);
            }
            return ServiceResult<MathResultDto>.Success(bestDiscount);
        }

        public async Task<ServiceResult<MathResultDto>> HandleTaxForSeller(decimal finalPrice)
        {
            _logger.LogInformation("Handling tax calculation for seller...");
            var adjustments = await _adjustmentRuleRepository.GetAllAsync();
            var classify = ClassifyAdjustment(adjustments);
            if (classify.Status == StatusResult.Errored)
            {
                _logger.LogError("Failed to classify tax rules.");
                return ServiceResult<MathResultDto>.Error("Không thể phân loại thuế.");
            }

            var taxes = classify.Data.Taxes.ToList();
            if (!taxes.Any() || taxes.Any(x => !x.IsPercentage))
            {
                _logger.LogError("Invalid or missing tax configuration.");
                return ServiceResult<MathResultDto>.Error("Không tìm thấy hoặc thuế không hợp lệ.");
            }

            return ServiceResult<MathResultDto>.Success(new MathResultDto
            {
                Value = taxes.Sum(x => x.Value),
                Amount = taxes.Sum(x => x.Value) * finalPrice,
                FinalAmount = finalPrice - (taxes.Sum(x => x.Value) * finalPrice),
            });
        }

        public async Task<ServiceResult<MathResultDto>> HandlePlatformFeeForSeller(decimal finalPrice)
        {
            _logger.LogInformation("Handling platform fee for seller...");
            var adjustments = (await _adjustmentRuleRepository.GetAllAsync()).Where(x => x.TargetLogic == TargetLogic.Auto);
            var classify = ClassifyAdjustment(adjustments);

            if (classify.Status == StatusResult.Errored)
            {
                _logger.LogError("Failed to classify platform fees.");
                return ServiceResult<MathResultDto>.Error("Không thể phân loại phí nền tảng.");
            }

            var platformFees = classify.Data.PlatformFees.ToList();
            if (platformFees.Count != 1)
            {
                _logger.LogError("Invalid number of platform fee rules: {Count}", platformFees.Count);
                return ServiceResult<MathResultDto>.Error("Cấu hình phí nền tảng không hợp lệ.");
            }

            var adjustment = platformFees.First();
            if (!adjustment.IsPercentage)
            {
                _logger.LogError("Platform fee is not percentage-based.");
                return ServiceResult<MathResultDto>.Error("Phí nền tảng phải là phần trăm.");
            }

            return ServiceResult<MathResultDto>.Success(new MathResultDto
            {
                AdjustmentRuleId = adjustment.RuleId,
                IsPercentage = adjustment.IsPercentage,
                Value = adjustment.Value,
                Amount = adjustment.Value * finalPrice,
                FinalAmount = finalPrice - (adjustment.Value * finalPrice),
            });
        }
    }
}
