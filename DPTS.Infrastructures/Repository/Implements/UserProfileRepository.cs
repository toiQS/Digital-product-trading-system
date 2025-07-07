using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class UserProfileRepository : IUserProfileRepository
    {
        private readonly ApplicationDbContext _context;

        public UserProfileRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Read

        public async Task<UserProfile?> GetByUserIdAsync(string userId)
        {
            return await _context.UserProfiles
                .Include(p => p.User)
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<bool> ExistsAsync(string userId)
        {
            return await _context.UserProfiles.AnyAsync(p => p.UserId == userId);
        }

        #endregion

        #region Create / Update / Delete

        public async Task AddAsync(UserProfile profile)
        {
            _context.UserProfiles.Add(profile);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserProfile profile)
        {
            _context.UserProfiles.Update(profile);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string userId)
        {
            var profile = await _context.UserProfiles.FindAsync(userId);
            if (profile != null)
            {
                _context.UserProfiles.Remove(profile);
                await _context.SaveChangesAsync();
            }
        }

        #endregion
    }
}
