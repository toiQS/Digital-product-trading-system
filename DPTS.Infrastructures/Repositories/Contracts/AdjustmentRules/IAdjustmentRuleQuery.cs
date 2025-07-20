using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.AdjustmentRules
{
    public interface IAdjustmentRuleQuery
    {
        Task<AdjustmentRule> GetByIdAsync(string ruleId, CancellationToken cancellationToken);
    }
}
