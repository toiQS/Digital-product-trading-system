using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class UserSecurityRepository : IUserSecurityRepository
    {
        private readonly ApplicationDbContext _context;

        public UserSecurityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(UserSecurity security)
        {
            _context.UserSecurities.Add(security);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(UserSecurity security)
        {
            _context.UserSecurities.Update(security);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string userId)
        {
            var entity = await _context.UserSecurities.FindAsync(userId);
            if (entity == null) return;
            _context.UserSecurities.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<UserSecurity?> GetByIdAsync(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId)) return null;

            return await _context.UserSecurities
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<bool> ExistsAsync(string userId)
        {
            return await _context.UserSecurities.AnyAsync(s => s.UserId == userId);
        }
    }

}
