using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Users
{
    public interface IUserQuery
    {
        Task<User?> GetByIdAsync(string userId);
        Task<User?> GetByEmailAsync(string email);
        Task<List<User>> GetUsersByRoleAsync(string roleId);
        Task<bool> IsEmailTakenAsync(string email);
        Task<bool> IsUsernameTakenAsync(string username);
    }
}
