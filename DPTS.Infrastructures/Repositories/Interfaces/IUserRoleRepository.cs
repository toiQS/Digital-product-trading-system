using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Interfaces
{
    public interface IUserRoleRepository
    {
        #region Query

        Task<UserRole?> GetByIdAsync(string roleId);
        Task<UserRole?> GetByNameAsync(string roleName);
        Task<List<UserRole>> GetAllAsync();
        Task<bool> IsRoleExistAsync(string roleName);

        #endregion

        #region Crud

        Task AddAsync(UserRole role);
        Task UpdateDescriptionAsync(string roleId, string newDescription);
        Task RemoveAsync(UserRole role);

        #endregion
    }
}
