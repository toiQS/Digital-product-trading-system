using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.UserSecurities
{
    public interface IUserSecurityCommand
    {
        Task AddAsync(UserSecurity security);
        Task UpdateAsync(UserSecurity security);
        Task RemoveAsync(UserSecurity security);
    }
}
