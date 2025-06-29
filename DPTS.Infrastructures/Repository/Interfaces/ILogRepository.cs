using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface ILogRepository
    {
        Task AddAsync(Log log);
        Task AddRangeAsync(IEnumerable<Log> logs);
        Task<IEnumerable<Log>> GetByTargetAsync(string targetType, string? targetId = null, DateTime? from = null, DateTime? to = null);
        Task<IEnumerable<Log>> GetByUserIdAsync(string userId, DateTime? from = null, DateTime? to = null, int skip = 0, int take = 50);
        Task<IEnumerable<Log>> GetRecentActionsAsync(int limit = 100);
    }
}