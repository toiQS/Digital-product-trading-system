using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IUserSecurityRepository
    {
        Task AddAsync(UserSecurity security);
        Task DeleteAsync(string userId);
        Task<bool> ExistsAsync(string userId);
        Task<UserSecurity?> GetByIdAsync(string userId);
        Task UpdateAsync(UserSecurity security);
    }
}