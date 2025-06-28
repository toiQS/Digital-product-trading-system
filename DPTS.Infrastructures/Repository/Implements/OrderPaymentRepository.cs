// OrderPaymentRepository - Refactored
using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class OrderPaymentRepository : IOrderPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderPaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(OrderPayment payment)
        {
            _context.OrderPayments.Add(payment);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(OrderPayment payment)
        {
            _context.OrderPayments.Update(payment);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string orderPaymentId)
        {
            var entity = await _context.OrderPayments.FindAsync(orderPaymentId);
            if (entity == null) return;
            _context.OrderPayments.Remove(entity);
            await _context.SaveChangesAsync();
        }

        public async Task<OrderPayment?> GetByIdAsync(string orderPaymentId)
        {
            if (string.IsNullOrWhiteSpace(orderPaymentId)) return null;

            return await _context.OrderPayments
                .AsNoTracking()
                .FirstOrDefaultAsync(p => p.OrderPaymentId == orderPaymentId);
        }

        public async Task<IEnumerable<OrderPayment>> GetsAsync(
            string? orderId = null,
            PaymentSourceType? sourceType = null,
            DateTime? from = null,
            DateTime? to = null,
            decimal? minAmount = null,
            decimal? maxAmount = null)
        {
            var query = _context.OrderPayments.AsQueryable();

            if (!string.IsNullOrWhiteSpace(orderId))
                query = query.Where(p => p.OrderId == orderId);

            if (sourceType.HasValue)
                query = query.Where(p => p.SourceType == sourceType);

            if (from.HasValue)
                query = query.Where(p => p.PaidAt >= from);

            if (to.HasValue)
                query = query.Where(p => p.PaidAt <= to);

            if (minAmount.HasValue)
                query = query.Where(p => p.Amount >= minAmount);

            if (maxAmount.HasValue)
                query = query.Where(p => p.Amount <= maxAmount);

            return await query
                .OrderByDescending(p => p.PaidAt)
                .AsNoTracking()
                .ToListAsync();
        }
    }
}