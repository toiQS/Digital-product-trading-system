using DPTS.Domains;
using DPTS.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;

namespace DPTS.Applications.Implements.repositories
{
    public class CategoryService
    {
        private readonly ApplicationDbContext _context;
        public CategoryService(ApplicationDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Category>> GetCategoriesAsync(string categoryName = null!)
        {
            var categories = await _context.Categories
                .Include(x => x.Products)
                .ToListAsync();
            if(categoryName != null)
            {
                categories.Where(x => x.CategoryName.ToLower().Contains(categoryName));
            }
            return categories;
        }
        public async Task<Category> GetCategoryAsync(string categoryId)
        {
            var category = await _context.Categories
                .Include(x => x.Products)
                .Where(x => x.CategoryId == categoryId)
                .FirstOrDefaultAsync();
            return category!;
        }
    }
}
