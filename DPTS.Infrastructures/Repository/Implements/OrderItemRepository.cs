using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class OrderItemRepository : IOrderItemRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<OrderItem>> GetsAsync(
            string? orderId = null,
            string? productId = null,
            decimal? minAmount = null,
            decimal? maxAmount = null,
            int? minQuantity = null,
            int? maxQuantity = null,
            bool includeProduct = false,
            bool includeOrder = false)
        {
            var query = _context.OrderItems.AsQueryable();

            if (!string.IsNullOrWhiteSpace(orderId))
                query = query.Where(oi => oi.OrderId == orderId);

            if (!string.IsNullOrWhiteSpace(productId))
                query = query.Where(oi => oi.ProductId == productId);

            if (minAmount != null)
                query = query.Where(oi => oi.TotalAmount >= minAmount);

            if (maxAmount != null)
                query = query.Where(oi => oi.TotalAmount <= maxAmount);

            if (minQuantity != null)
                query = query.Where(oi => oi.Quantity >= minQuantity);

            if (maxQuantity != null)
                query = query.Where(oi => oi.Quantity <= maxQuantity);

            if (includeProduct)
                query = query.Include(oi => oi.Product);

            if (includeOrder)
                query = query.Include(oi => oi.Order);

            return await query.ToListAsync();
        }

        public async Task<OrderItem?> GetByIdAsync(
            string id,
            bool includeProduct = false,
            bool includeOrder = false)
        {
            var query = _context.OrderItems.Where(oi => oi.OrderItemId == id);

            if (includeProduct)
                query = query.Include(oi => oi.Product);

            if (includeOrder)
                query = query.Include(oi => oi.Order);

            return await query.FirstOrDefaultAsync();
        }

        public async Task AddAsync(OrderItem item)
        {
            _context.OrderItems.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(OrderItem item)
        {
            _context.OrderItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var item = await _context.OrderItems.FindAsync(id);
            if (item != null)
            {
                _context.OrderItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }
    }
}
