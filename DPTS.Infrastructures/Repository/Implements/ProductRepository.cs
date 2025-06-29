using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class ProductRepository : IProductRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Create / Update / Delete

        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string productId)
        {
            var product = await _context.Products.FindAsync(productId);
            if (product != null)
            {
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        #endregion

        #region Read

        public async Task<Product?> GetByIdAsync(string productId, bool includeCategory = false, bool includeStore = false, bool includeAdjustments = false)
        {
            if (string.IsNullOrWhiteSpace(productId))
                return null;

            var query = _context.Products.AsQueryable();

            if (includeCategory)
                query = query.Include(p => p.Category);

            if (includeStore)
                query = query.Include(p => p.Store);

            if (includeAdjustments)
                query = query.Include(p => p.ProductAdjustments);

            return await query.FirstOrDefaultAsync(p => p.ProductId == productId);
        }

        public async Task<IEnumerable<Product>> GetByStoreAsync(string storeId, ProductStatus? status = null, int skip = 0, int take = 50)
        {
            var query = _context.Products
                .Where(p => p.StoreId == storeId);

            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);

            return await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> SearchAsync(string? keyword = null, string? categoryId = null, ProductStatus? status = ProductStatus.Available, int skip = 0, int take = 50)
        {
            var query = _context.Products.AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
                query = query.Where(p => p.ProductName.Contains(keyword) || p.Summary.Contains(keyword));

            if (!string.IsNullOrWhiteSpace(categoryId))
                query = query.Where(p => p.CategoryId == categoryId);

            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);

            return await query
                .OrderByDescending(p => p.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<int> CountByStoreAsync(string storeId, ProductStatus? status = null)
        {
            var query = _context.Products.Where(p => p.StoreId == storeId);

            if (status.HasValue)
                query = query.Where(p => p.Status == status.Value);

            return await query.CountAsync();
        }

        #endregion
    }
}
