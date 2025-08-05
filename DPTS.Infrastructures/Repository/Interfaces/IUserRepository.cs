using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task DeleteAsync(string userId);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> ExistsAsync(string userId);
        Task<IEnumerable<User>> GetAllAsync();
        Task<User?> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(string userId, bool includeRelated = true);
        Task<IEnumerable<User>> GetByIdsAsync(List<string> contactIds);
        Task<User?> GetByUsernameAsync(string username);
        Task UpdateAsync(User user);
        Task<bool> UsernameExistsAsync(string username);
    }
}