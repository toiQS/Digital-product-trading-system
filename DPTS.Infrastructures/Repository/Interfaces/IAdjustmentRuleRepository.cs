using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IAdjustmentRuleRepository
    {
        Task AddAsync(AdjustmentRule rule);
        Task DeleteAsync(string ruleId);
        Task<bool> ExistsAsync(string ruleId);
        Task<IEnumerable<AdjustmentRule>> GetAllAsync(AdjustmentType? type = null, RuleStatus? status = null);
        Task<AdjustmentRule?> GetByIdAsync(string ruleId);
        Task<AdjustmentRule?> GetDefaultAsync(AdjustmentType type);
        Task<AdjustmentRule?> GetLatestByTypeAsync(AdjustmentType type);
        Task UpdateAsync(AdjustmentRule rule);
    }
}