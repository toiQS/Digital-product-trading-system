using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Users
{
    public interface IUserQuery
    {
        Task<User?> GetByIdAsync(string userId, CancellationToken cancellationToken);
    }
}
