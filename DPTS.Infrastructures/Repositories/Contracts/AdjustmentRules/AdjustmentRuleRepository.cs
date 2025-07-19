using DPTS.Domains;
using DPTS.Infrastructures.Datas;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repositories.Contracts.AdjustmentRules
{
    public class AdjustmentRuleRepository : IAdjustmentRuleQuery, IAdjustmentRuleCommand
    {
        private readonly ApplicationDbContext _context;
        public AdjustmentRuleRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<AdjustmentRule?> GetByIdAsync(string ruleId, CancellationToken cancellationToken)
        {
            return await _context.Adjustments.FirstOrDefaultAsync(x => x.RuleId == ruleId, cancellationToken) ;
        }
    }
}
