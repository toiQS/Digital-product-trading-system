using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Interfaces
{
    public interface ILogActionRepository
    {
        #region Query

        Task<List<LogAction>> GetByUserIdAsync(string userId, int limit = 50);

        Task<List<LogAction>> GetRecentByTargetAsync(string targetType, string targetId, int limit = 50);

        Task<List<LogAction>> GetRecentSystemActionsAsync(int limit = 100);

        Task<List<LogAction>> SearchAsync(string? keyword, DateTime? from, DateTime? to);

        #endregion

        #region Crud

        Task AddAsync(LogAction log);

        #endregion
    }
}
