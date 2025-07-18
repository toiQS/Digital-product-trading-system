using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.LogActions
{
    public interface ILogActionCommand
    {
        Task AddAsync(LogAction logAction);
        Task RemoveAsync(LogAction logAction);
        Task UpdateAsync(LogAction logAction);
    }
}
