using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class EscrowProcessRepository : IEscrowProcessRepository
    {
        private readonly ApplicationDbContext _context;

        public EscrowProcessRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<EscrowProcess>> GetByEscrowIdAsync(string escrowId)
        {
            if (string.IsNullOrWhiteSpace(escrowId))
                return Enumerable.Empty<EscrowProcess>();

            return await _context.EscrowProcesses
                .Where(p => p.EscrowId == escrowId)
                .OrderByDescending(p => p.ProcessAt)
                .ToListAsync();
        }

        public async Task<EscrowProcess?> GetByIdAsync(string processId)
        {
            if (string.IsNullOrWhiteSpace(processId))
                return null;

            return await _context.EscrowProcesses
                .Include(p => p.Escrow)
                .FirstOrDefaultAsync(p => p.ProcessId == processId);
        }

        public async Task AddAsync(EscrowProcess process)
        {
            if (process is null)
                throw new ArgumentNullException(nameof(process));

            _context.EscrowProcesses.Add(process);
            await _context.SaveChangesAsync();
        }
    }
}
