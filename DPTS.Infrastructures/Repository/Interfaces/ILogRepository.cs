namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface ILogRepository
    {
        Task AddAsync(Log log);
        Task AddManyAsync(IEnumerable<Log> logs);
        Task<IEnumerable<Log>> GetAllAsync(int take = 100);
        Task<IEnumerable<Log>> GetByTargetAsync(string targetType, string targetId);
        Task<IEnumerable<Log>> GetByUserIdAsync(string userId, int take = 100);
    }
}