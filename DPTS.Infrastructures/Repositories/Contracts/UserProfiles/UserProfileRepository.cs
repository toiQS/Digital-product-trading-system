using DPTS.Domains;
using DPTS.Infrastructures.Datas;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repositories.Contracts.UserProfiles
{
    public class UserProfileRepository : IUserProfileQuery, IUserProfileCommand
    {
        private readonly ApplicationDbContext _dbContext;
        public UserProfileRepository(ApplicationDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
        }
        public Task<UserProfile?> GetByIdAsync(string userId, CancellationToken cancellationToken)
        {
            return _dbContext.UserProfiles.FirstOrDefaultAsync(x => x.UserId == userId, cancellationToken);
        }
    }
}
