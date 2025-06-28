using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class PaymentMethodRepository : IPaymentMethodRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentMethodRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(PaymentMethod method)
        {
            _context.PaymentMethods.Add(method);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(PaymentMethod method)
        {
            _context.PaymentMethods.Update(method);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string methodId)
        {
            var entity = await _context.PaymentMethods.FindAsync(methodId);
            if (entity == null) return;
            _context.PaymentMethods.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<PaymentMethod?> GetByIdAsync(string methodId)
        {
            if (string.IsNullOrWhiteSpace(methodId)) return null;

            return await _context.PaymentMethods
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.PaymentMethodId == methodId);
        }

        public async Task<IEnumerable<PaymentMethod>> GetsAsync(
            string? userId = null,
            PaymentProvider? provider = null,
            bool? isVerified = null,
            bool? isDefault = null)
        {
            var query = _context.PaymentMethods.AsQueryable();

            if (!string.IsNullOrWhiteSpace(userId))
                query = query.Where(p => p.UserId == userId);

            if (provider.HasValue)
                query = query.Where(p => p.Provider == provider);

            if (isVerified.HasValue)
                query = query.Where(p => p.IsVerified == isVerified);

            if (isDefault.HasValue)
                query = query.Where(p => p.IsDefault == isDefault);

            return await query
                .OrderByDescending(p => p.IsDefault)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
