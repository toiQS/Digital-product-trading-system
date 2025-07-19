using DPTS.Domains;
using DPTS.Infrastructures.Datas;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repositories.Contracts.Escrows
{
    public class EscrowRepository : IEscrowCommand, IEscrowQuery
    {
        private readonly ApplicationDbContext _context;
        public EscrowRepository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        public async Task<IEnumerable<Escrow>> GetDoneEscrowsAsync(CancellationToken cancellationToken)
        {
            var query = _context.Escrows.AsQueryable();
            query = query.Where(e => e.Status == EscrowStatus.Avalible);
            return await query.ToListAsync(cancellationToken);
        }
    }
}
