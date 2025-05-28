using DPTS.Applications.Shareds.Interfaces;
using DPTS.Infrastructures.Data;

namespace DPTS.Applications.Shareds.Implements
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private readonly Dictionary<Type, object> _repositories = new();

        public UnitOfWork(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public IRepository<TEntity> Repository<TEntity>() where TEntity : class
        {
            var entityType = typeof(TEntity);
            if (!_repositories.ContainsKey(entityType))
            {
                var repositoryInstance = new Repository<TEntity>(_context);
                _repositories[entityType] = repositoryInstance;
            }

            return (IRepository<TEntity>)_repositories[entityType];
        }

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            return await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task ExecuteTransactionAsync(Func<Task> transactionOperations, CancellationToken cancellationToken = default)
        {
            if (transactionOperations == null)
                throw new ArgumentNullException(nameof(transactionOperations), "Transaction operations cannot be null.");

            await using var transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            try
            {
                await transactionOperations();
                await _context.SaveChangesAsync(cancellationToken);
                await transaction.CommitAsync(cancellationToken);
            }
            catch
            {
                await transaction.RollbackAsync(cancellationToken);
                throw;
            }
        }
    }
}
