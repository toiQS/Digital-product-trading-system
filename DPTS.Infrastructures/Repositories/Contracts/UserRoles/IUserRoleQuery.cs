using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.UserRoles
{
    public interface IUserRoleQuery
    {
        Task<UserRole?> GetByIdAsync(string roleId);
        Task<UserRole?> GetByNameAsync(string roleName);
        Task<List<UserRole>> GetAllAsync();
        Task<bool> IsRoleExistAsync(string roleName);
    }
}
