using DPTS.Domains;

namespace DPTS.Infrastructures.Repositories.Contracts.Stores
{
    public interface IStoreQueryRepository
    {
        Task<Store?> GetByIdAsync(string storeId);
        Task<Store?> GetByUserIdAsync(string userId);
        Task<List<Store>> GetActiveStoresAsync();
        Task<List<Store>> SearchAsync(StoreSearchCriteria criteria);
    }
    public class StoreSearchCriteria
    {
        public string? Keyword { get; set; }
        public StoreStatus? Status { get; set; }
    }

}
