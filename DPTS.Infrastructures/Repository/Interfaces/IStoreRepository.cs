using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IStoreRepository
    {
        Task AddAsync(Store store);
        Task DeleteAsync(string storeId);
        Task<bool> ExistsAsync(string storeId);
        Task<Store?> GetByIdAsync(string storeId, bool includeUser = false);
        Task<Store?> GetByUserIdAsync(string userId, bool includeUser = false);
        Task<IEnumerable<Store>> GetsAsync(string? userId = null, string? storeName = null, StoreStatus? status = null, DateTime? from = null, DateTime? to = null, bool includeUser = false);
        Task UpdateAsync(Store store);
    }
}