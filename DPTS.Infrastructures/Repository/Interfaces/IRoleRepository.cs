using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IRoleRepository
    {
        Task AddAsync(Role role);
        Task DeleteAsync(string roleId);
        Task<Role?> GetByIdAsync(string roleId, bool includeUsers = false);
        Task<Role?> GetByNameAsync(string roleName, bool includeUsers = false);
        Task<IEnumerable<Role>> GetsAsync(string? search = null, string? roleName = null, bool includeUsers = false);
        Task UpdateAsync(Role role);
    }
}