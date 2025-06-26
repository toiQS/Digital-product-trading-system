using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class TaxRepository : ITaxRepository
    {
        private readonly ApplicationDbContext _context;

        public TaxRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Tax>> GetsAsync(string? text = null, bool includeCategory = false)
        {
            var query = _context.Taxs.AsQueryable();

            if (includeCategory)
                query = query.Include(t => t.Category);

            if (!string.IsNullOrWhiteSpace(text))
            {
                query = query.Where(t =>
                    EF.Functions.Like(t.TaxName, $"%{text}%") ||
                    EF.Functions.Like(t.TaxDescription, $"%{text}%"));
            }

            return await query.ToListAsync();
        }

        public async Task<Tax?> GetByIdAsync(string id, bool includeCategory = false)
        {
            if (string.IsNullOrWhiteSpace(id))
                return null;

            var query = _context.Taxs.AsQueryable();

            query = query.Where(t => t.TaxId == id);

            if (includeCategory)
                query = query.Include(t => t.Category);

            return await query.FirstOrDefaultAsync();
        }

        public async Task AddAsync(Tax tax)
        {
            _context.Taxs.Add(tax);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Tax tax)
        {
            _context.Taxs.Update(tax);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var tax = await _context.Taxs.FindAsync(id);
            if (tax != null)
            {
                _context.Taxs.Remove(tax);
                await _context.SaveChangesAsync();
            }
        }
    }
}
