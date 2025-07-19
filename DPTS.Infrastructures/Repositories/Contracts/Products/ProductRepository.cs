
using DPTS.Domains;
using DPTS.Infrastructures.Datas;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repositories.Contracts.Products
{
    public class ProductRepository : IProductQuery, IProductCommand
    {
        private readonly ApplicationDbContext _context;
        public ProductRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<Product?> GetByIdAsync(string productId, CancellationToken cancellationToken)
        {
            return await _context.Products
                .FirstOrDefaultAsync(p => p.ProductId == productId, cancellationToken);
           
        }
    }
}
