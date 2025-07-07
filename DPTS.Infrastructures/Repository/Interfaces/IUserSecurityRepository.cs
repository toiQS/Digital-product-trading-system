using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IUserSecurityRepository
    {
        Task AddAsync(UserSecurity security);
        Task<bool> ExistsAsync(string userId);
        Task<UserSecurity?> GetByUserIdAsync(string userId);
        Task IncrementFailedLoginAsync(string userId);
        Task ResetFailedLoginAsync(string userId);
        Task SetLockoutAsync(string userId, DateTime until);
        Task UpdateAsync(UserSecurity security);
    }
}