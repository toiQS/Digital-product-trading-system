using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface ICategoryRepository
    {
        Task AddAsync(Category category);
        Task DeleteAsync(string categoryId);
        Task<bool> ExistsAsync(string categoryId);
        Task<Category?> GetByIdAsync(string categoryId, bool includeProduct = false, bool includeAdjustmentRule = false);
        Task<IEnumerable<Category>> GetsAsync(string? text = null, bool includeProduct = false, bool includeAdjustmentRule = false);
        Task UpdateAsync(Category category);
    }
}