using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class TradeRepository : ITradeRepository
    {
        private readonly ApplicationDbContext _context;

        public TradeRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Trade>> GetsAsync(
            string? search = null,
            TradeStatus? status = null,
            string? userId = null,
            DateTime? from = null,
            DateTime? to = null,
            bool includeWallets = false,
            bool includeUser = false)
        {
            var query = _context.Trades.AsQueryable();

            if (!string.IsNullOrWhiteSpace(search))
            {
                var lowered = search.ToLower();
                query = query.Where(t =>
                    t.TradeName.ToLower().Contains(lowered) ||
                    t.TradeIcon.ToLower().Contains(lowered) ||
                    t.TradeFromId.ToLower().Contains(lowered) ||
                    t.TradeToId.ToLower().Contains(lowered));
            }

            if (status.HasValue)
                query = query.Where(t => t.Status == status.Value);

            if (!string.IsNullOrWhiteSpace(userId))
                query = query.Where(t => t.UserId == userId);

            if (from.HasValue)
                query = query.Where(t => t.TradeDate >= from.Value);

            if (to.HasValue)
                query = query.Where(t => t.TradeDate <= to.Value);

            if (includeWallets)
            {
                query = query
                    .Include(t => t.TradeFrom)
                    .Include(t => t.TradeTo);
            }

            if (includeUser)
                query = query.Include(t => t.User);

            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<Trade?> GetByIdAsync(string tradeId, bool includeWallets = false, bool includeUser = false)
        {
            if (string.IsNullOrWhiteSpace(tradeId))
                return null;

            var query = _context.Trades.AsQueryable();

            if (includeWallets)
            {
                query = query
                    .Include(t => t.TradeFrom)
                    .Include(t => t.TradeTo);
            }

            if (includeUser)
                query = query.Include(t => t.User);

            return await query.FirstOrDefaultAsync(t => t.TradeId == tradeId);
        }

        public async Task AddAsync(Trade trade)
        {
            if (trade == null)
                throw new ArgumentNullException(nameof(trade));

            _context.Trades.Add(trade);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Trade trade)
        {
            if (trade == null)
                throw new ArgumentNullException(nameof(trade));

            _context.Trades.Update(trade);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string tradeId)
        {
            if (string.IsNullOrWhiteSpace(tradeId))
                return;

            var trade = await _context.Trades.FindAsync(tradeId);
            if (trade != null)
            {
                _context.Trades.Remove(trade);
                await _context.SaveChangesAsync();
            }
        }
    }

}