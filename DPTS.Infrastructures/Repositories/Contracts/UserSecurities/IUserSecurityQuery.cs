using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Users
{
    public interface IUserSecurityQuery
    {
        Task<UserSecurity?> GetByUserIdAsync(string userId);
        Task<bool> IsLockedOutAsync(string userId);
        Task<DateTime?> GetLockoutUntilAsync(string userId);
        Task<bool> IsEmailVerifiedAsync(string userId);
        Task<bool> IsTwoFactorEnabledAsync(string userId);
    }
}
