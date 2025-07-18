using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Users
{
    public interface IUserCommand
    {
        Task AddAsync(User user);
        Task UpdateAsync(User user);
        Task DeactivateUserAsync(string userId);
        Task DeleteAsync(User user);
    }
}
