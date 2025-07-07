using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class EscrowRepository : IEscrowRepository
    {
        private readonly ApplicationDbContext _context;

        public EscrowRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Query Builder
        private IQueryable<Escrow> BuildBaseQuery(
            bool includeOrder = false,
            bool includeStore = false)
        {
            var query = _context.Escrows.AsQueryable();

            if (includeOrder)
                query = query.Include(e => e.Order);

            if (includeStore)
                query = query.Include(e => e.Store);

            return query;
        }
        #endregion

        #region Get Methods

        public async Task<IEnumerable<Escrow>> GetAllAsync(
            EscrowStatus? status = null,
            string? storeId = null,
            bool includeOrder = false,
            bool includeStore = false)
        {
            var query = BuildBaseQuery(includeOrder, includeStore);

            if (status.HasValue)
                query = query.Where(e => e.Status == status.Value);

            if (!string.IsNullOrWhiteSpace(storeId))
                query = query.Where(e => e.StoreId == storeId);

            return await query
                .OrderByDescending(e => e.CreatedAt)
                .ToListAsync();
        }

        public async Task<Escrow?> GetByIdAsync(string escrowId, bool includeOrder = false, bool includeStore = false)
        {
            if (string.IsNullOrWhiteSpace(escrowId))
                return null;

            return await BuildBaseQuery(includeOrder, includeStore)
                .FirstOrDefaultAsync(e => e.EscrowId == escrowId);
        }

        public async Task<Escrow?> GetByOrderIdAsync(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                return null;

            return await _context.Escrows
                .FirstOrDefaultAsync(e => e.OrderId == orderId);
        }

        public async Task<IEnumerable<Escrow>> GetExpiredAsync(DateTime? before = null)
        {
            var now = before ?? DateTime.UtcNow;
            return await _context.Escrows
                .Where(e => e.Expired <= now && e.Status == EscrowStatus.WaitingConfirm)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(string escrowId)
        {
            return await _context.Escrows.AnyAsync(e => e.EscrowId == escrowId);
        }

        public async Task<decimal> GetTotalHeldAmountAsync(string storeId)
        {
            return await _context.Escrows
                .Where(e => e.StoreId == storeId && e.Status == EscrowStatus.WaitingConfirm)
                .SumAsync(e => e.Amount - e.PlatformFeeAmount);
        }

        #endregion

        #region CRUD

        public async Task AddAsync(Escrow escrow)
        {
            _context.Escrows.Add(escrow);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Escrow escrow)
        {
            _context.Escrows.Update(escrow);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string escrowId)
        {
            var escrow = await _context.Escrows.FindAsync(escrowId);
            if (escrow != null)
            {
                _context.Escrows.Remove(escrow);
                await _context.SaveChangesAsync();
            }
        }

        #endregion
    }
}
