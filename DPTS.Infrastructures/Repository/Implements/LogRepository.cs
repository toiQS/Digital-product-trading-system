using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class LogRepository : ILogRepository
    {
        private readonly ApplicationDbContext _context;

        public LogRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Log>> GetsAsync(
            string? userId = null,
            string? actionKeyword = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            bool includeUser = false)
        {
            var query = _context.Logs.AsQueryable();

            if (!string.IsNullOrWhiteSpace(userId))
                query = query.Where(l => l.UserId == userId);

            if (!string.IsNullOrWhiteSpace(actionKeyword))
                query = query.Where(l => l.Action.Contains(actionKeyword));

            if (fromDate.HasValue)
                query = query.Where(l => l.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(l => l.CreatedAt <= toDate.Value);

            if (includeUser)
                query = query.Include(l => l.User);

            return await query
                .OrderByDescending(l => l.CreatedAt)
                .ToListAsync();
        }

        public async Task<Log?> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;

            return await _context.Logs
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.LogId == id);
        }

        public async Task AddAsync(Log log)
        {
            _context.Logs.Add(log);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var log = await _context.Logs.FindAsync(id);
            if (log != null)
            {
                _context.Logs.Remove(log);
                await _context.SaveChangesAsync();
            }
        }
    }

}