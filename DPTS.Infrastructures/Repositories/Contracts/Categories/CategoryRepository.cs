using DPTS.Domains;
using DPTS.Infrastructures.Datas;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DPTS.Infrastructures.Repositories.Contracts.Categories
{
    public class CategoryRepository : ICategoryQuery
    {
        private readonly ApplicationDbContext _context;

        public CategoryRepository(ApplicationDbContext context)
        {
            _context = context;
        }
        private IQueryable<Category> Base()
        {
            return _context.Categories.AsQueryable();
        }
        public async Task<IEnumerable<Category>> GetCategoriesAsync(bool includeProduct, bool isAvailible, bool sortByCountProductAvalible, int take, CancellationToken cancellationToken)
        {
            var query = Base();
            if (includeProduct)
            {
                query = query.Include(c => c.Products);
            }
            if (isAvailible)
            {
                query = query.SelectMany(x => x.Products.Where(p => p.Status == ProductStatus.Available).DefaultIfEmpty(), (c, p) => new { Category = c, Product = p })
                             .Where(x => x.Product != null)
                             .Select(x => x.Category);
            }
            if (sortByCountProductAvalible)
            {
                query = query.OrderByDescending(c => c.Products.Count(p => p.Status == ProductStatus.Available));
            }
            if (take > 0)
            {
                
                query = query.Take(take);
            }
            return await query.ToListAsync(cancellationToken);
        }
    }
}
