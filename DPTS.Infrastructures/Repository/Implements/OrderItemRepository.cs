using DPTS.Domains;
using DPTS.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class OrderItemRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderItemRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        #region Get Methods

        public async Task<OrderItem?> GetByIdAsync(string orderItemId, bool includeProduct = false)
        {
            if (string.IsNullOrWhiteSpace(orderItemId))
                return null;

            var query = _context.OrderItems.AsQueryable();

            if (includeProduct)
                query = query.Include(oi => oi.Product);

            return await query.FirstOrDefaultAsync(oi => oi.OrderItemId == orderItemId);
        }

        public async Task<IEnumerable<OrderItem>> GetByOrderIdAsync(string orderId, bool includeProduct = false)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                return Enumerable.Empty<OrderItem>();

            var query = _context.OrderItems.Where(oi => oi.OrderId == orderId);

            if (includeProduct)
                query = query.Include(oi => oi.Product);

            return await query.ToListAsync();
        }

        public async Task<IEnumerable<OrderItem>> GetByProductIdAsync(string productId)
        {
            return await _context.OrderItems
                .Where(oi => oi.ProductId == productId)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalFinalByOrderIdAsync(string orderId)
        {
            return await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .SumAsync(oi => oi.FinalPrice);
        }

        public async Task<int> GetTotalQuantitySoldAsync(string productId)
        {
            return await _context.OrderItems
                .Where(oi => oi.ProductId == productId)
                .SumAsync(oi => oi.Quantity);
        }

        #endregion

        #region CRUD

        public async Task AddAsync(OrderItem item)
        {
            _context.OrderItems.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task AddRangeAsync(IEnumerable<OrderItem> items)
        {
            _context.OrderItems.AddRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteByOrderIdAsync(string orderId)
        {
            var items = await _context.OrderItems
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();

            _context.OrderItems.RemoveRange(items);
            await _context.SaveChangesAsync();
        }

        #endregion
    }
}
