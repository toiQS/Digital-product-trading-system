using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IRoleRepository
    {
        Task AddAsync(Role role);
        Task DeleteAsync(string roleId);
        Task<bool> ExistsAsync(string roleId);
        Task<IEnumerable<Role>> GetAllAsync();
        Task<Role?> GetByIdAsync(string roleId);
        Task<Role?> GetByNameAsync(string roleName);
        Task UpdateAsync(Role role);
    }
}