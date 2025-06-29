using DPTS.Domains;
using DPTS.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class UserSecurityRepository
    {
        private readonly ApplicationDbContext _context;

        public UserSecurityRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Read

        public async Task<UserSecurity?> GetByUserIdAsync(string userId)
        {
            return await _context.UserSecurities
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<bool> ExistsAsync(string userId)
        {
            return await _context.UserSecurities.AnyAsync(s => s.UserId == userId);
        }

        #endregion

        #region Create / Update

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

        #endregion

        #region Login Control

        public async Task IncrementFailedLoginAsync(string userId)
        {
            var security = await _context.UserSecurities.FindAsync(userId);
            if (security != null)
            {
                security.FailedLoginAttempts++;
                await _context.SaveChangesAsync();
            }
        }

        public async Task ResetFailedLoginAsync(string userId)
        {
            var security = await _context.UserSecurities.FindAsync(userId);
            if (security != null)
            {
                security.FailedLoginAttempts = 0;
                security.LockoutUntil = null;
                await _context.SaveChangesAsync();
            }
        }

        public async Task SetLockoutAsync(string userId, DateTime until)
        {
            var security = await _context.UserSecurities.FindAsync(userId);
            if (security != null)
            {
                security.LockoutUntil = until;
                await _context.SaveChangesAsync();
            }
        }

        #endregion
    }
}
