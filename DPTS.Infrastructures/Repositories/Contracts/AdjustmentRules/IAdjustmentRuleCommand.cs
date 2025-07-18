using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.AdjustmentRules
{
    public interface IAdjustmentRuleCommand
    {
        // Cập nhật số lượt sử dụng (dùng trong transactional boundary)
        Task IncrementVoucherUsageAsync(string ruleId = default!,
                                        string userId = null!);
        Task AddAsync(AdjustmentRule rule);

        Task UpdateStatusAsync(string ruleId, RuleStatus newStatus);

    }
}
