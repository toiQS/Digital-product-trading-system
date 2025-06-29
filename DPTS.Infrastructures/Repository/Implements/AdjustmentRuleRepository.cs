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

        public async Task<IEnumerable<AdjustmentRule>> GetAllAsync(AdjustmentType? type = null, RuleStatus? status = null)
        {
            var query = _context.AdjustmentRules.AsQueryable();

            if (type.HasValue)
                query = query.Where(r => r.Type == type.Value);

            if (status.HasValue)
                query = query.Where(r => r.Status == status.Value);

            return await query
                .OrderByDescending(r => r.Version)
                .ThenByDescending(r => r.From)
                .ToListAsync();
        }

        public async Task<AdjustmentRule?> GetByIdAsync(string ruleId)
        {
            if (string.IsNullOrWhiteSpace(ruleId))
                return null;

            return await _context.AdjustmentRules
                .FirstOrDefaultAsync(r => r.RuleId == ruleId);
        }

        public async Task<AdjustmentRule?> GetDefaultAsync(AdjustmentType type)
        {
            return await _context.AdjustmentRules
                .Where(r => r.Type == type && r.IsDefaultForNewProducts && r.Status == RuleStatus.Active)
                .OrderByDescending(r => r.Version)
                .FirstOrDefaultAsync();
        }

        public async Task<AdjustmentRule?> GetLatestByTypeAsync(AdjustmentType type)
        {
            return await _context.AdjustmentRules
                .Where(r => r.Type == type)
                .OrderByDescending(r => r.Version)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> ExistsAsync(string ruleId)
        {
            return await _context.AdjustmentRules.AnyAsync(r => r.RuleId == ruleId);
        }

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
    }
}
