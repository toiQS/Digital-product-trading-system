using DPTS.Domains;

namespace DPTS.Infrastructures.Repository.Interfaces
{
    public interface ITradeRepository
    {
        Task AddAsync(Trade trade);
        Task DeleteAsync(string tradeId);
        Task<Trade?> GetByIdAsync(string tradeId, bool includeWallets = false, bool includeUser = false);
        Task<IEnumerable<Trade>> GetsAsync(string? search = null, TradeStatus? status = null, string? userId = null, DateTime? from = null, DateTime? to = null, bool includeWallets = false, bool includeUser = false);
        Task UpdateAsync(Trade trade);
    }
}