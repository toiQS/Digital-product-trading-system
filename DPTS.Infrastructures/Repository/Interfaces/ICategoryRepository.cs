using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface ICategoryRepository
    {
        Task AddAsync(Category category);
        Task DeleteAsync(string id);
        Task<Category?> GetByIdAsync(string id, bool includeProduct = false);
        Task<IEnumerable<Category>> GetsAsync(string? text = null, bool includeProduct = true);
        Task UpdateAsync(Category category);
    }
}