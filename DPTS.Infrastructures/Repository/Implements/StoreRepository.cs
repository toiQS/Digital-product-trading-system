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

        #region Read

        public async Task<Store?> GetByIdAsync(string storeId)
        {
            return await _context.Stores
                .Include(s => s.User)
                .FirstOrDefaultAsync(s => s.StoreId == storeId);
        }

        public async Task<Store?> GetByUserIdAsync(string userId)
        {
            return await _context.Stores
                .FirstOrDefaultAsync(s => s.UserId == userId);
        }

        public async Task<IEnumerable<Store>> GetByStatusAsync(StoreStatus status)
        {
            return await _context.Stores
                .Where(s => s.Status == status)
                .OrderBy(s => s.CreateAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<Store>> GetAllAsync()
        {
            return await _context.Stores
                .Include(s => s.User)
                .OrderByDescending(s => s.CreateAt)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(string storeId)
        {
            return await _context.Stores.AnyAsync(s => s.StoreId == storeId);
        }

        #endregion

        #region Create / Update / Delete

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

        #endregion
    }
}
