using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IWalletRepository
    {
        Task AddAsync(Wallet wallet);
        Task DeleteAsync(Wallet wallet);
        Task<bool> ExistsAsync(string walletId);
        Task<Wallet?> GetByIdAsync(string walletId, bool includeUser = false);
        Task<Wallet?> GetByUserIdAsync(string userId, bool includeUser = false);
        Task<IEnumerable<Wallet>> GetsAsync(UnitCurrency? currency = null, double? minBalance = null, double? maxBalance = null, string? userKeyword = null, bool includeUser = false);
        Task UpdateAsync(Wallet wallet);
    }
}