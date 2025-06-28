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
            var entity = await _context.UserProfiles.FindAsync(userId);
            if (entity == null) return;
            _context.UserProfiles.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<UserProfile?> GetByIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return null;

            return await _context.UserProfiles
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.UserId == userId);
        }

        public async Task<IEnumerable<UserProfile>> GetsAsync(
            string? keyword = null,
            string? phone = null)
        {
            var query = _context.UserProfiles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var lowered = keyword.ToLower();
                query = query.Where(p =>
                    p.FullName!.ToLower().Contains(lowered) ||
                    p.User.Username.ToLower().Contains(lowered) ||
                    p.User.Email.ToLower().Contains(lowered));
            }

            if (!string.IsNullOrWhiteSpace(phone))
                query = query.Where(p => p.Phone!.Contains(phone));

            return await query
                .Include(p => p.User)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(string userId)
        {
            return await _context.UserProfiles.AnyAsync(p => p.UserId == userId);
        }
    }

}
