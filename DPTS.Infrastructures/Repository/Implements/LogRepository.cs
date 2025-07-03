using DPTS.Domains;
using DPTS.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class LogRepository
    {
        private readonly ApplicationDbContext _context;

        public LogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Create

        public async Task AddAsync(Log log)
        {
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task AddManyAsync(IEnumerable<Log> logs)
        {
            _context.Logs.AddRange(logs);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Read

        public async Task<IEnumerable<Log>> GetAllAsync(int take = 100)
        {
            return await _context.Logs
                .Include(l => l.User)
                .OrderByDescending(l => l.CreatedAt)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<Log>> GetByUserIdAsync(string userId, int take = 100)
        {
            return await _context.Logs
                .Where(l => l.UserId == userId)
                .OrderByDescending(l => l.CreatedAt)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<Log>> GetByTargetAsync(string targetType, string targetId)
        {
            return await _context.Logs
                .Where(l => l.TargetType == targetType && l.TargetId == targetId)
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        #endregion
    }
}
