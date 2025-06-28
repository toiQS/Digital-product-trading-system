using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IUserProfileRepository
    {
        Task AddAsync(UserProfile profile);
        Task DeleteAsync(string userId);
        Task<bool> ExistsAsync(string userId);
        Task<UserProfile?> GetByIdAsync(string userId);
        Task<IEnumerable<UserProfile>> GetsAsync(string? keyword = null, string? phone = null);
        Task UpdateAsync(UserProfile profile);
    }
}