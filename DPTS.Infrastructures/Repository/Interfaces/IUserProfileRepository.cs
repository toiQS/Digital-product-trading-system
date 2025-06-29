using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IUserProfileRepository
    {
        Task AddAsync(UserProfile profile);
        Task DeleteAsync(string userId);
        Task<bool> ExistsAsync(string userId);
        Task<UserProfile?> GetByUserIdAsync(string userId);
        Task UpdateAsync(UserProfile profile);
    }
}