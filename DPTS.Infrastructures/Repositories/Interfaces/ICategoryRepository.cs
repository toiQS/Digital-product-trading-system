using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        #region Query

        Task<Category?> GetByIdAsync(string categoryId, bool includeProducts = false);

        Task<List<Category>> GetAllAsync(bool includeInactive = false);

        Task<List<Category>> GetFeaturedAsync();

        Task<bool> ExistsAsync(string categoryName);

        #endregion

        #region Crud

        Task AddAsync(Category category);
        Task UpdateAsync(Category category);
        Task RemoveAsync(Category category);

        #endregion
    }
}
