using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class UserRepository : IUserRepository
    {
        private readonly ApplicationDbContext _context;

        public UserRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<User?> GetByIdAsync(
            string userId,
            bool includeWallet = false,
            bool includeRole = false)
        {
            if (string.IsNullOrWhiteSpace(userId)) return null;

            var query = _context.Users.AsQueryable();

            if (includeWallet)
                query = query.Include(u => u.Wallet);

            if (includeRole)
                query = query.Include(u => u.Role);

            return await query
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.UserId == userId);
        }

        public async Task<User?> GetByEmailAsync(string email, bool includeRole = false)
        {
            if (string.IsNullOrWhiteSpace(email)) return null;

            var query = _context.Users.AsQueryable();

            if (includeRole)
                query = query.Include(u => u.Role);

            return await query
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByUsernameAsync(string username, bool includeRole = false)
        {
            if (string.IsNullOrWhiteSpace(username)) return null;

            var query = _context.Users.AsQueryable();

            if (includeRole)
                query = query.Include(u => u.Role);

            return await query
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<IEnumerable<User>> GetsAsync(
            string? search = null,
            string? roleId = null,
            bool? twoFactor = null,
            DateTime? from = null,
            DateTime? to = null,
            bool includeRole = false,
            int? pageIndex = null,
            int? pageSize = null)
        {
            var query = _context.Users.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowered = search.ToLower();
                query = query.Where(u =>
                    u.Username.ToLower().Contains(lowered) ||
                    u.Email.ToLower().Contains(lowered) ||
                    (u.FullName != null && u.FullName.ToLower().Contains(lowered)) ||
                    (u.Phone != null && u.Phone.Contains(lowered)));
            }

            if (!string.IsNullOrWhiteSpace(roleId))
                query = query.Where(u => u.RoleId == roleId);

            if (twoFactor.HasValue)
                query = query.Where(u => u.TwoFactorEnabled == twoFactor.Value);

            if (from.HasValue)
                query = query.Where(u => u.CreatedAt >= from.Value);

            if (to.HasValue)
                query = query.Where(u => u.CreatedAt <= to.Value);

            if (includeRole)
                query = query.Include(u => u.Role);

            if (pageIndex.HasValue && pageSize.HasValue)
                query = query.Skip(pageIndex.Value * pageSize.Value).Take(pageSize.Value);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<bool> ExistsAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return false;
            return await _context.Users.AnyAsync(u => u.UserId == userId);
        }

        public async Task AddAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(User user)
        {
            if (user == null) throw new ArgumentNullException(nameof(user));
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(User user)
        {
            if (user == null) return;
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
