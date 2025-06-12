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

        public async Task<IEnumerable<Escrow>> GetsAsync(
            string? sellerId = null,
            EscrowStatus? status = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            bool includeOrder = false,
            bool includeSeller = false)
        {
            var query = _context.Escrows.AsQueryable();

            // Ưu tiên lọc theo seller
            if (!string.IsNullOrWhiteSpace(sellerId))
                query = query.Where(e => e.SellerId == sellerId);

            // Lọc theo trạng thái
            if (status.HasValue)
                query = query.Where(e => e.Status == status.Value);

            if (fromDate.HasValue)
                query = query.Where(e => e.CreatedAt >= fromDate.Value);

            if (toDate.HasValue)
                query = query.Where(e => e.CreatedAt <= toDate.Value);

            // Điều kiện include
            if (includeOrder) query = query.Include(e => e.Order);
            if (includeSeller) query = query.Include(e => e.Seller);

            return await query.ToListAsync();
        }

        public async Task<Escrow?> GetByIdAsync(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return null;

            return await _context.Escrows
                .Include(e => e.Order)
                .Include(e => e.Seller)
                .FirstOrDefaultAsync(e => e.EscrowId == id);
        }

        public async Task<Escrow?> GetByOrderIdAsync(string orderId)
        {
            if (string.IsNullOrWhiteSpace(orderId)) return null;

            return await _context.Escrows
                .Include(e => e.Order)
                .Include(e => e.Seller)
                .FirstOrDefaultAsync(e => e.OrderId == orderId);
        }

        public async Task<IEnumerable<Escrow>> GetBySellerIdAsync(string sellerId)
        {
            if (string.IsNullOrWhiteSpace(sellerId)) return Enumerable.Empty<Escrow>();

            return await _context.Escrows
                .Where(e => e.SellerId == sellerId)
                .Include(e => e.Order)
                .ToListAsync();
        }

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

        public async Task DeleteAsync(string id)
        {
            var escrow = await _context.Escrows.FindAsync(id);
            if (escrow != null)
            {
                _context.Escrows.Remove(escrow);
                await _context.SaveChangesAsync();
            }
        }
    }
}
