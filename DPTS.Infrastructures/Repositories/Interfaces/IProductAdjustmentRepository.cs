using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Interfaces
{
    public interface IProductAdjustmentRepository
    {
        #region Query

        Task<List<AdjustmentRule>> GetRulesByProductIdAsync(string productId);

        Task<List<Product>> GetProductsByRuleIdAsync(string ruleId);

        Task<bool> IsRuleLinkedToProductAsync(string productId, string ruleId);

        #endregion

        #region Crud

        Task AddAsync(ProductAdjustment productAdjustment);
        Task RemoveAsync(ProductAdjustment productAdjustment);

        Task RemoveAllByProductIdAsync(string productId);
        Task RemoveAllByRuleIdAsync(string ruleId);

        #endregion
    }
}
