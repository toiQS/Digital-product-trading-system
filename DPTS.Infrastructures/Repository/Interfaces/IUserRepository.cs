using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IUserRepository
    {
        Task AddAsync(User user);
        Task DeleteAsync(User user);
        Task<bool> ExistsAsync(string userId);
        Task<User?> GetByEmailAsync(string email, bool includeRole = false);
        Task<User?> GetByIdAsync(string userId, bool includeWallet = false, bool includeRole = false);
        Task<User?> GetByUsernameAsync(string username, bool includeRole = false);
        Task<IEnumerable<User>> GetsAsync(string? search = null, string? roleId = null, bool? twoFactor = null, DateTime? from = null, DateTime? to = null, bool includeRole = false, int? pageIndex = null, int? pageSize = null);
        Task UpdateAsync(User user);
    }
}