using DPTS.Infrastructures.Data;

namespace DPTS.Applications.Shareds.Interfaces
{
    public interface IUnitOfWork
    {
        Task ExecuteTransactionAsync(Func<Task> transactionOperations, CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        IRepository<TEntity> Repository<TEntity>() where TEntity : class;
    }
}
