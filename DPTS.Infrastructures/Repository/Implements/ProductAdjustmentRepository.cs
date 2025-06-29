using DPTS.Domains;
using DPTS.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class ProductAdjustmentRepository
    {
        private readonly ApplicationDbContext _context;

        public ProductAdjustmentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Create / Delete

        public async Task AddAsync(ProductAdjustment adjustment)
        {
            _context.ProductAdjustments.Add(adjustment);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<ProductAdjustment> adjustments)
        {
            _context.ProductAdjustments.AddRange(adjustments);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByProductIdAsync(string productId)
        {
            var list = await _context.ProductAdjustments
                .Where(pa => pa.ProductId == productId)
                .ToListAsync();

            _context.ProductAdjustments.RemoveRange(list);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string productId, string ruleId)
        {
            var item = await _context.ProductAdjustments
                .FirstOrDefaultAsync(pa => pa.ProductId == productId && pa.RuleId == ruleId);

            if (item != null)
            {
                _context.ProductAdjustments.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        #endregion

        #region Read

        public async Task<IEnumerable<AdjustmentRule>> GetRulesByProductIdAsync(string productId)
        {
            return await _context.ProductAdjustments
                .Where(pa => pa.ProductId == productId)
                .Include(pa => pa.Rule)
                .Select(pa => pa.Rule)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByRuleIdAsync(string ruleId)
        {
            return await _context.ProductAdjustments
                .Where(pa => pa.RuleId == ruleId)
                .Include(pa => pa.Product)
                .Select(pa => pa.Product)
                .ToListAsync();
        }

        public async Task<bool> HasAdjustmentAsync(string productId)
        {
            return await _context.ProductAdjustments
                .AnyAsync(pa => pa.ProductId == productId);
        }

        #endregion
    }
}
