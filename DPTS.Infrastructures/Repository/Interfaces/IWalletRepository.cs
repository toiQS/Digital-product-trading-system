using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface IWalletRepository
    {
        Task AddAsync(Wallet wallet);
        Task UpdateAsync(Wallet wallet);
        Task DeleteAsync(string walletId);
        Task<Wallet?> GetByIdAsync(string walletId);
        
        Task<IEnumerable<Wallet>> GetsAsync(
            string? userId = null,
            decimal? minBalance = null,
            decimal? maxBalance = null
        );
    }

}