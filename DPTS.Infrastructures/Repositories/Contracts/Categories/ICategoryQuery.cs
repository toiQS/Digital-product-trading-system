using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Categories
{
    public interface ICategoryQuery
    {
        Task<Category?> GetByIdAsync(string categoryId, bool includeProducts = false);
        Task<List<Category>> GetAllAsync(bool includeInactive = false);
        Task<List<Category>> GetFeaturedAsync();
        Task<bool> ExistsAsync(string categoryName);
    }

}
