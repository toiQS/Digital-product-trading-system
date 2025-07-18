using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Stores
{
    public interface IStoreCommandRepository
    {
        Task AddAsync(Store store);
        Task UpdateAsync(Store store);
        Task RemoveAsync(Store store);
    }

}
