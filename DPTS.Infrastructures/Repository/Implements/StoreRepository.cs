using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class StoreRepository : IStoreRepository
    {
        private readonly ApplicationDbContext _context;

        public StoreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Store?> GetByIdAsync(string storeId, bool includeUser = false)
        {
            if (string.IsNullOrWhiteSpace(storeId)) return null;

            var query = _context.Stores.AsQueryable();

            if (includeUser)
                query = query.Include(s => s.User);

            return await query.FirstOrDefaultAsync(s => s.StoreId == storeId);
        }

        public async Task<Store?> GetByUserIdAsync(string userId, bool includeUser = false)
        {
            if (string.IsNullOrWhiteSpace(userId)) return null;

            var query = _context.Stores.AsQueryable();

            if (includeUser)
                query = query.Include(s => s.User);

            return await query.FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<IEnumerable<Store>> GetsAsync(
            string? userId = null,
            string? storeName = null,
            StoreStatus? status = null,
            DateTime? from = null,
            DateTime? to = null,
            bool includeUser = false)
        {
            var query = _context.Stores.AsQueryable();

            if (!string.IsNullOrWhiteSpace(userId))
                query = query.Where(s => s.UserId == userId);

            if (!string.IsNullOrWhiteSpace(storeName))
                query = query.Where(s => s.StoreName.Contains(storeName));

            if (status.HasValue)
                query = query.Where(s => s.Status == status.Value);

            if (from.HasValue)
                query = query.Where(s => s.CreateAt >= from.Value);

            if (to.HasValue)
                query = query.Where(s => s.CreateAt <= to.Value);

            if (includeUser)
                query = query.Include(s => s.User);

            return await query
                .OrderByDescending(s => s.CreateAt)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(string storeId)
        {
            if (string.IsNullOrWhiteSpace(storeId)) return false;
            return await _context.Stores.AnyAsync(s => s.StoreId == storeId);
        }

        public async Task AddAsync(Store store)
        {
            _context.Stores.Add(store);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Store store)
        {
            _context.Stores.Update(store);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string storeId)
        {
            var store = await _context.Stores.FindAsync(storeId);
            if (store != null)
            {
                _context.Stores.Remove(store);
                await _context.SaveChangesAsync();
            }
        }
    }
}
