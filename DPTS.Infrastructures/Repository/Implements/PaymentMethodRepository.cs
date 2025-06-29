using DPTS.Domains;
using DPTS.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class PaymentMethodRepository
    {
        private readonly ApplicationDbContext _context;

        public PaymentMethodRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Create / Update / Delete

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
            var method = await _context.PaymentMethods.FindAsync(methodId);
            if (method != null)
            {
                _context.PaymentMethods.Remove(method);
                await _context.SaveChangesAsync();
            }
        }

        #endregion

        #region Read

        public async Task<PaymentMethod?> GetByIdAsync(string methodId)
        {
            return await _context.PaymentMethods.FindAsync(methodId);
        }

        public async Task<IEnumerable<PaymentMethod>> GetByUserIdAsync(string userId)
        {
            return await _context.PaymentMethods
                .Where(p => p.UserId == userId)
                .OrderByDescending(p => p.IsDefault)
                .ToListAsync();
        }

        public async Task<PaymentMethod?> GetDefaultByUserIdAsync(string userId)
        {
            return await _context.PaymentMethods
                .FirstOrDefaultAsync(p => p.UserId == userId && p.IsDefault);
        }

        public async Task<bool> IsProviderLinkedAsync(string userId, PaymentProvider provider)
        {
            return await _context.PaymentMethods
                .AnyAsync(p => p.UserId == userId && p.Provider == provider);
        }

        public async Task<IEnumerable<PaymentMethod>> GetVerifiedByUserIdAsync(string userId)
        {
            return await _context.PaymentMethods
                .Where(p => p.UserId == userId && p.IsVerified)
                .ToListAsync();
        }

        #endregion
    }
}
