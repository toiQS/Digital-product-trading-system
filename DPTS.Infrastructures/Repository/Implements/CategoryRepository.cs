using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region QueryBuilder
        private IQueryable<Category> BuildBaseQuery(bool includeProduct = false, bool includeAdjustmentRule = false)
        {
            var query = _context.Categories.AsQueryable();

            if (includeProduct)
                query = query.Include(c => c.Products);


            return query;
        }
        #endregion

        #region Gets
        public async Task<IEnumerable<Category>> GetsAsync(string? text = null, bool includeProduct = false, bool includeAdjustmentRule = false)
        {
            var query = BuildBaseQuery(includeProduct, includeAdjustmentRule);

            if (!string.IsNullOrWhiteSpace(text))
                query = query.Where(c => EF.Functions.Like(c.CategoryName, $"%{text}%"));

            return await query
                .OrderBy(c => c.DisplayOrder)
                .ThenBy(c => c.CategoryName)
                .ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(string categoryId, bool includeProduct = false, bool includeAdjustmentRule = false)
        {
            if (string.IsNullOrWhiteSpace(categoryId))
                return null;

            return await BuildBaseQuery(includeProduct, includeAdjustmentRule)
                .FirstOrDefaultAsync(c => c.CategoryId == categoryId);
        }

        public async Task<bool> ExistsAsync(string categoryId)
        {
            if (string.IsNullOrWhiteSpace(categoryId))
                return false;

            return await _context.Categories.AnyAsync(c => c.CategoryId == categoryId);
        }
        #endregion

        #region CRUD
        public async Task AddAsync(Category category)
        {
            if (category is null)
                throw new ArgumentNullException(nameof(category));

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            if (category is null)
                throw new ArgumentNullException(nameof(category));

            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string categoryId)
        {
            if (string.IsNullOrWhiteSpace(categoryId))
                return;

            var category = await _context.Categories.FindAsync(categoryId);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
        #endregion
    }
}
