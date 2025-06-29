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

        public async Task AddRangeAsync(IEnumerable<Log> logs)
        {
            _context.Logs.AddRange(logs);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Read

        public async Task<IEnumerable<Log>> GetByUserIdAsync(
            string userId,
            DateTime? from = null,
            DateTime? to = null,
            int skip = 0,
            int take = 50)
        {
            var query = _context.Logs.AsQueryable()
                .Where(l => l.UserId == userId);

            if (from.HasValue)
                query = query.Where(l => l.CreatedAt >= from.Value);

            if (to.HasValue)
                query = query.Where(l => l.CreatedAt <= to.Value);

            return await query
                .OrderByDescending(l => l.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<Log>> GetByTargetAsync(
            string targetType,
            string? targetId = null,
            DateTime? from = null,
            DateTime? to = null)
        {
            var query = _context.Logs.AsQueryable()
                .Where(l => l.TargetType == targetType);

            if (!string.IsNullOrWhiteSpace(targetId))
                query = query.Where(l => l.TargetId == targetId);

            if (from.HasValue)
                query = query.Where(l => l.CreatedAt >= from.Value);

            if (to.HasValue)
                query = query.Where(l => l.CreatedAt <= to.Value);

            return await query
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Log>> GetRecentActionsAsync(int limit = 100)
        {
            return await _context.Logs
                .OrderByDescending(l => l.CreatedAt)
                .Take(limit)
                .ToListAsync();
        }

        #endregion
    }
}
