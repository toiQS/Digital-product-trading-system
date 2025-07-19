using DPTS.Domains;
using DPTS.Infrastructures.Datas;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repositories.Contracts.Stores
{
    public class StoreRepository : IStoreQuery, IStoreCommand
    {
        private readonly ApplicationDbContext _context;

        public StoreRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<Store?> GetByIdAsync(string storeId, CancellationToken cancellationToken)
        {
           return await _context.Stores
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.StoreId == storeId, cancellationToken);
        }
    }
}
