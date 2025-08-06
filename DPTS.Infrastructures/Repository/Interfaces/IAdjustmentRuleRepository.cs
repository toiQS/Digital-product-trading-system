using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IAdjustmentRuleRepository
    {
        Task AddAsync(AdjustmentRule rule);
        Task DeleteAsync(string ruleId);
        Task<IEnumerable<AdjustmentRule>> GetActiveRulesAsync(AdjustmentType? type = null, AdjustmentScope? scope = null);
        Task<IEnumerable<AdjustmentRule>> GetAdjustmentRulesByStoreIdAsync(string storeId);
        Task<IEnumerable<AdjustmentRule>> GetAllAsync();
        Task<AdjustmentRule?> GetByIdAsync(string ruleId);
        Task<AdjustmentRule?> GetByVoucherCodeAsync(string code);
        Task UpdateAsync(AdjustmentRule rule);
    }
}