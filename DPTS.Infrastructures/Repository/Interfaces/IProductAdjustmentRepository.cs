using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IProductAdjustmentRepository
    {
        Task AddAsync(ProductAdjustment adjustment);
        Task AddRangeAsync(IEnumerable<ProductAdjustment> adjustments);
        Task DeleteAsync(string productId, string ruleId);
        Task DeleteByProductIdAsync(string productId);
        Task<IEnumerable<Product>> GetProductsByRuleIdAsync(string ruleId);
        Task<IEnumerable<AdjustmentRule>> GetRulesByProductIdAsync(string productId);
        Task<bool> HasAdjustmentAsync(string productId);
    }
}