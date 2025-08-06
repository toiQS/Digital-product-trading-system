using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class AdjustmentRuleRepository : IAdjustmentRuleRepository
    {
        private readonly ApplicationDbContext _context;

        public AdjustmentRuleRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Read

        public async Task<AdjustmentRule?> GetByIdAsync(string ruleId)
        {
            return await _context.AdjustmentRules
                .Include(r => r.ProductAdjustments)
                .FirstOrDefaultAsync(r => r.RuleId == ruleId);
        }

        public async Task<IEnumerable<AdjustmentRule>> GetAllAsync()
        {
            return await _context.AdjustmentRules.ToListAsync();
        }

        public async Task<IEnumerable<AdjustmentRule>> GetActiveRulesAsync(AdjustmentType? type = null, AdjustmentScope? scope = null)
        {
            var now = DateTime.UtcNow;

            var query = _context.AdjustmentRules
                .Where(r => r.Status == RuleStatus.Active &&
                            (!r.From.HasValue || r.From <= now) &&
                            (!r.To.HasValue || r.To >= now));

            if (type.HasValue)
                query = query.Where(r => r.Type == type.Value);

            if (scope.HasValue)
                query = query.Where(r => r.Scope == scope.Value);

            return await query.ToListAsync();
        }

        public async Task<AdjustmentRule?> GetByVoucherCodeAsync(string code)
        {
            var now = DateTime.UtcNow;
            return await _context.AdjustmentRules
                .FirstOrDefaultAsync(r =>
                    r.VoucherCode == code &&
                    r.Status == RuleStatus.Active &&
                    (!r.From.HasValue || r.From <= now) &&
                    (!r.To.HasValue || r.To >= now));
        }

        #endregion

        #region Write

        public async Task AddAsync(AdjustmentRule rule)
        {
            _context.AdjustmentRules.Add(rule);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AdjustmentRule rule)
        {
            _context.AdjustmentRules.Update(rule);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string ruleId)
        {
            var rule = await _context.AdjustmentRules.FindAsync(ruleId);
            if (rule != null)
            {
                _context.AdjustmentRules.Remove(rule);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<AdjustmentRule>> GetAdjustmentRulesByStoreIdAsync(string storeId)
        {
            var result =await _context.AdjustmentRules
                .Where(r => r.SourceId == storeId && r.Source == AdjustmentSource.Seller)
                .ToListAsync();
            return result;
        }

        #endregion
    }
}
