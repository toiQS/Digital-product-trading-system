using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Interfaces
{
    public interface IStoreRepository
    {
        #region Query
        Task<Store?> GetByIdAsync(string storeId);
        Task<Store?> GetByUserIdAsync(string userId);
        Task<List<Store>> GetActiveStoresAsync();
        Task<List<Store>> SearchByNameAsync(string keyword);
        Task<bool> IsStoreOwnerAsync(string storeId, string userId);
        #endregion

        #region Crud
        Task AddAsync(Store store);
        Task UpdateAsync(Store store);
        Task RemoveAsync(Store store);
        #endregion
    }
}
