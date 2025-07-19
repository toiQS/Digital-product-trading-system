using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.UserProfiles
{
    public interface IUserProfileQuery
    {
        Task<UserProfile?> GetByIdAsync(string userId, CancellationToken cancellationToken);
    }
}
