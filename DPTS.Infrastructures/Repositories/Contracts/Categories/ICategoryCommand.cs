using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Categories
{
    public interface ICategoryCommand
    {
        Task AddAsync(Category category);
        Task RemoveAsync(Category category);
        Task UpdateAsync(Category category);
        Task CheckExisted(Category category);
    }
}
