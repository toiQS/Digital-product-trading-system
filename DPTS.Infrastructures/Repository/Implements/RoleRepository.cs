using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class RoleRepository : IRoleRepository
    {
        private readonly ApplicationDbContext _context;

        public RoleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Role>> GetsAsync(
            string? search = null,
            string? roleName = null,
            bool includeUsers = false)
        {
            var query = _context.Roles.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowered = search.ToLowerInvariant();
                query = query.Where(r =>
                    r.RoleId.ToLower().Contains(lowered) ||
                    r.RoleName.ToLower().Contains(lowered) ||
                    r.Description.ToLower().Contains(lowered));
            }

            if (!string.IsNullOrWhiteSpace(roleName))
            {
                var lowered = roleName.ToLowerInvariant();
                query = query.Where(r => r.RoleName.ToLower() == lowered);
            }

            if (includeUsers)
                query = query.Include(r => r.Users);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<Role?> GetByIdAsync(string roleId, bool includeUsers = false)
        {
            if (string.IsNullOrWhiteSpace(roleId))
                return null;

            var query = _context.Roles.AsQueryable();

            if (includeUsers)
                query = query.Include(r => r.Users);

            return await query.FirstOrDefaultAsync(r => r.RoleId == roleId);
        }

        public async Task<Role?> GetByNameAsync(string roleName, bool includeUsers = false)
        {
            if (string.IsNullOrWhiteSpace(roleName))
                return null;

            var lowered = roleName.ToLowerInvariant();
            var query = _context.Roles.AsQueryable();

            if (includeUsers)
                query = query.Include(r => r.Users);

            return await query.FirstOrDefaultAsync(r => r.RoleName.ToLower() == lowered);
        }

        public async Task AddAsync(Role role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            _context.Roles.Add(role);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Role role)
        {
            if (role == null)
                throw new ArgumentNullException(nameof(role));

            _context.Roles.Update(role);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string roleId)
        {
            if (string.IsNullOrWhiteSpace(roleId))
                return;

            var role = await _context.Roles.FindAsync(roleId);
            if (role != null)
            {
                _context.Roles.Remove(role);
                await _context.SaveChangesAsync();
            }
        }
    }

}