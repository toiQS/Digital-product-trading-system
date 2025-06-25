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

        public async Task<IEnumerable<Category>> GetsAsync(string? text = null, bool includeProduct = true, bool includeTax = false)
        {
            var query = _context.Categories.AsQueryable();

            if (includeProduct)
                query = query.Include(c => c.Products);

            if (includeTax)
                query = query.Include(c => c.Taxes);

            if (!string.IsNullOrWhiteSpace(text))
            {
                query = query.Where(c => EF.Functions.Like(c.CategoryName, $"%{text}%"));
            }

            return await query.ToListAsync();
        }

        public async Task<Category?> GetByIdAsync(string id, bool includeProduct = false, bool includeTax = false)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            var query = _context.Categories.AsQueryable();

            query = query.Where(x => x.CategoryId == id);

            if (includeProduct)
                query = query.Include(x => x.Products);

            if (includeTax)
                query = query.Include(x => x.Taxes);

            return await query.FirstOrDefaultAsync();
        }


        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }
    }
}
