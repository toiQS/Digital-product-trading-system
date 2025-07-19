using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Stores
{
    public interface IStoreQuery
    {
        Task<Store?> GetByIdAsync(string storeId, CancellationToken cancellationToken);
    }
}
