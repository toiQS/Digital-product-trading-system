using DPTS.Domains;
using DPTS.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class OrderPaymentRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderPaymentRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Create

        public async Task AddAsync(OrderPayment payment)
        {
            _context.OrderPayments.Add(payment);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<OrderPayment> payments)
        {
            _context.OrderPayments.AddRange(payments);
            await _context.SaveChangesAsync();
        }

        #endregion

        #region Read

        public async Task<IEnumerable<OrderPayment>> GetByOrderIdAsync(string orderId)
        {
            return await _context.OrderPayments
                .Where(p => p.OrderId == orderId)
                .OrderBy(p => p.PaidAt)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalPaidByOrderIdAsync(string orderId)
        {
            return await _context.OrderPayments
                .Where(p => p.OrderId == orderId)
                .SumAsync(p => p.Amount);
        }

        public async Task<IEnumerable<OrderPayment>> GetByWalletIdAsync(string walletId, DateTime? from = null, DateTime? to = null)
        {
            var query = _context.OrderPayments
                .Where(p => p.SourceType == PaymentSourceType.Wallet && p.WalletId == walletId);

            if (from.HasValue)
                query = query.Where(p => p.PaidAt >= from.Value);
            if (to.HasValue)
                query = query.Where(p => p.PaidAt <= to.Value);

            return await query.OrderByDescending(p => p.PaidAt).ToListAsync();
        }

        public async Task<IEnumerable<OrderPayment>> GetByPaymentMethodIdAsync(string paymentMethodId, DateTime? from = null, DateTime? to = null)
        {
            var query = _context.OrderPayments
                .Where(p => p.SourceType == PaymentSourceType.PaymentMethod && p.PaymentMethodId == paymentMethodId);

            if (from.HasValue)
                query = query.Where(p => p.PaidAt >= from.Value);
            if (to.HasValue)
                query = query.Where(p => p.PaidAt <= to.Value);

            return await query.OrderByDescending(p => p.PaidAt).ToListAsync();
        }

        #endregion
    }
}
