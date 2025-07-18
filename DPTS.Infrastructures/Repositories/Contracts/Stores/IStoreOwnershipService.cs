namespace DPTS.Infrastructures.Repositories.Contracts.Stores
{
    public interface IStoreOwnershipService
    {
        Task<bool> IsStoreOwnerAsync(string storeId, string userId);
    }

}
