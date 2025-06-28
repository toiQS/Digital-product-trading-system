using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;
        public UserRepository(ApplicationDbContext context) => _context = context;

        public async Task AddAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string userId)
        {
            var entity = await _context.Users.FindAsync(userId);
            if (entity == null) return;
            _context.Users.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<User?> GetByIdAsync(string userId, bool includeProfile = false, bool includeSecurity = false, bool includeRole = false)
        {
            var query = _context.Users.AsQueryable();

            if (includeProfile) query = query.Include(u => u.Profile);
            if (includeSecurity) query = query.Include(u => u.Security);
            if (includeRole) query = query.Include(u => u.Role);

            return await query
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }
        public async Task<User?> GetByEmailAsync(string email, bool includeProfile = false, bool includeSecurity = false, bool includeRole = false)
        {
            var query = _context.Users.AsQueryable();

            if (includeProfile) query = query.Include(u => u.Profile);
            if (includeSecurity) query = query.Include(u => u.Security);
            if (includeRole) query = query.Include(u => u.Role);

            return await query
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<IEnumerable<User>> GetsAsync(
            string? keyword = null,
            string? roleId = null,
            DateTime? createdFrom = null,
            DateTime? createdTo = null,
            bool includeProfile = false,
            bool includeSecurity = false,
            bool includeRole = false)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                var lowered = keyword.ToLower();
                query = query.Where(u =>
                    u.Username.ToLower().Contains(lowered) ||
                    u.Email.ToLower().Contains(lowered));
            }

            if (!string.IsNullOrWhiteSpace(roleId))
                query = query.Where(u => u.RoleId == roleId);

            if (createdFrom.HasValue)
                query = query.Where(u => u.CreatedAt >= createdFrom.Value);

            if (createdTo.HasValue)
                query = query.Where(u => u.CreatedAt <= createdTo.Value);

            if (includeProfile) query = query.Include(u => u.Profile);
            if (includeSecurity) query = query.Include(u => u.Security);
            if (includeRole) query = query.Include(u => u.Role);

            return await query
                .OrderByDescending(u => u.CreatedAt)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(string userId)
        {
            return await _context.Users.AnyAsync(u => u.UserId == userId);
        }
    }

}
