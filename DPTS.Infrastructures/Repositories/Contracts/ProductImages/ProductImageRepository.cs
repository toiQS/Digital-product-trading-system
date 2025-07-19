using DPTS.Domains;
using DPTS.Infrastructures.Datas;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repositories.Contracts.ProductImages
{
    public class ProductImageRepository : IProductImageQuery, IProductImageCommand
    {
        private readonly ApplicationDbContext _context;
        public ProductImageRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<ProductImage>> GetByProductIdAsync(string productId, CancellationToken cancellationToken)
        {
            return await _context.ProductImages
                .Where(pi => pi.ProductId == productId)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<ProductImage?> GetPrimaryImageByProductIdAsync(string productId, CancellationToken cancellationToken)
        {
            return await _context.ProductImages
                .Where(pi => pi.ProductId == productId && pi.IsPrimary)
                .FirstOrDefaultAsync(cancellationToken);
        }
    }
}
