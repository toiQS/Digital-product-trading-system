using DPTS.Domains;
using DPTS.Infrastructures.Datas;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repositories.Contracts.ProductAdjustments
{
    public class ProductAdjustmentRepository : IProductAdjustmentQuery, IProductAdjustmentCommand
    {
        private readonly ApplicationDbContext _context;
        public ProductAdjustmentRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<IEnumerable<ProductAdjustment>> GetByProductIdAsync(string productId, CancellationToken cancellationToken)
        {
            return await _context.ProductAdjustments
                .Where(pa => pa.ProductId == productId)
                .ToListAsync(cancellationToken);
        }
    }
}
