using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.UserProfiles
{
    public interface IUserProfileQuery
    {
        Task<UserProfile?> GetByUserIdAsync(string userId);
        Task<bool> ExistsAsync(string userId);
    }
}
