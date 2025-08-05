using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IStoreRepository
    {
        Task AddAsync(Store store);
        Task DeleteAsync(string storeId);
        Task<bool> ExistsAsync(string storeId);
        Task<IEnumerable<Store>> GetAllAsync();
        Task<Store?> GetByIdAsync(string storeId);
        Task<IEnumerable<Store>> GetByStatusAsync(StoreStatus status);
        Task<Store?> GetByUserIdAsync(string userId);
        Task<IEnumerable<Store>> GetByIdsAsync(List<string> contactIds);
        Task UpdateAsync(Store store);
    }
}