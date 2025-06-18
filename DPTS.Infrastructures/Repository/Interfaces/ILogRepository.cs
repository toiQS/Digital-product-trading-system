using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface ILogRepository
    {
        Task AddAsync(Log log);
        Task DeleteAsync(string id);
        Task<Log?> GetByIdAsync(string id);
        Task<IEnumerable<Log>> GetsAsync(string? userId = null, string? actionKeyword = null, DateTime? fromDate = null, DateTime? toDate = null, bool includeUser = false);
    }
}