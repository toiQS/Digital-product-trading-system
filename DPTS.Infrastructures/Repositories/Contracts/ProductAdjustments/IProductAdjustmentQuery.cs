using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.ProductAdjustments
{
    public interface IProductAdjustmentQuery
    {
        Task<List<AdjustmentRule>> GetRulesByProductIdAsync(string productId);

        Task<List<Product>> GetProductsByRuleIdAsync(string ruleId);

        Task<bool> IsRuleLinkedToProductAsync(string productId, string ruleId);
    }

}
