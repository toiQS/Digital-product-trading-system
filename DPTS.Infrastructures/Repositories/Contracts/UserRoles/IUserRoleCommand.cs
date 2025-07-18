using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Users
{
    public interface IUserRoleCommand
    {
        Task AddAsync(UserRole role);
        Task UpdateDescriptionAsync(string roleId, string newDescription);
        Task RemoveAsync(UserRole role);
    }
}
