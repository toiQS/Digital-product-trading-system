using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task DeleteAsync(string userId);
        Task<bool> ExistsAsync(string userId);
        Task<User?> GetByIdAsync(string userId, bool includeProfile = false, bool includeSecurity = false, bool includeRole = false);
        Task<User?> GetByEmailAsync(string email, bool includeProfile = false, bool includeSecurity = false, bool includeRole = false);
        Task<IEnumerable<User>> GetsAsync(string? keyword = null, string? roleId = null, DateTime? createdFrom = null, DateTime? createdTo = null, bool includeProfile = false, bool includeSecurity = false, bool includeRole = false);
        Task UpdateAsync(User user);
    }
}