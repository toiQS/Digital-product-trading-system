using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.ProductAdjustments
{
    public interface IProductAdjustmentCommand
    {
        Task AddAsync(ProductAdjustment productAdjustment);
        Task RemoveAsync(ProductAdjustment productAdjustment);

        Task RemoveAllByProductIdAsync(string productId);
        Task RemoveAllByRuleIdAsync(string ruleId);
    }

}
