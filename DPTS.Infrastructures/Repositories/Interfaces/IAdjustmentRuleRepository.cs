using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Interfaces
{
    public interface IAdjustmentRuleRepository
    {
        #region Query
        Task<AdjustmentRule?> GetByIdAsync(string? ruleId, bool? includeProductAdjustment);
        Task<List<AdjustmentRule>> GetActiveRulesAsync(DateTime asOf);

        // Dùng cho áp dụng giảm giá/thuế theo loại
        Task<List<AdjustmentRule>> GetApplicableRulesAsync(
            AdjustmentType type,
            string sellerId,
            decimal orderAmount,
            DateTime asOf);

        // Dùng cho kiểm tra voucher nhập vào
        Task<AdjustmentRule?> GetByVoucherCodeAsync(string code, DateTime asOf);

        // Kiểm tra giới hạn sử dụng voucher tổng và theo người dùng
        Task<bool> IsVoucherUsageAllowedAsync(string ruleId, string userId);

        // Cập nhật số lượt sử dụng (dùng trong transactional boundary)
        Task IncrementVoucherUsageAsync(string ruleId, string userId);

        // Tìm rule theo scope (áp dụng toàn đơn hay từng sản phẩm)
        Task<List<AdjustmentRule>> GetRulesByScopeAsync(AdjustmentScope scope, DateTime asOf);

        // Lọc các rule thuộc Platform hoặc Seller
        Task<List<AdjustmentRule>> GetRulesBySourceAsync(AdjustmentSource source, string sellerId);

        // Logic validate điều kiện từ JSON - thường nên delegate ra service riêng
        Task<bool> EvaluateRuleConditionAsync(AdjustmentRule rule, string userId, Dictionary<string, object> contextData);
        #endregion
        #region Crud
        Task AddAsync(AdjustmentRule rule);
        Task RemoveAsync(AdjustmentRule rule);
        Task UpdateAsync(AdjustmentRule rule);
        #endregion
    }
}
