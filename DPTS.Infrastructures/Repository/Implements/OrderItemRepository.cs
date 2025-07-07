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

        #region Read

        public async Task<IEnumerable<OrderItem>> GetByOrderIdAsync(string orderId)
        {
            return await _context.OrderItems
                .Include(oi => oi.Product)
                .Where(oi => oi.OrderId == orderId)
                .ToListAsync();
        }

        public async Task<OrderItem?> GetByIdAsync(string orderItemId)
        {
            return await _context.OrderItems
                .Include(oi => oi.Product)
                .Include(oi => oi.Order)
                .FirstOrDefaultAsync(oi => oi.OrderItemId == orderItemId);
        }

        #endregion

        #region Write

        public async Task AddAsync(OrderItem item)
        {
            _context.OrderItems.Add(item);
            await _context.SaveChangesAsync();
        }

        public async Task AddManyAsync(IEnumerable<OrderItem> items)
        {
            _context.OrderItems.AddRange(items);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(OrderItem item)
        {
            _context.OrderItems.Update(item);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string orderItemId)
        {
            var item = await _context.OrderItems.FindAsync(orderItemId);
            if (item != null)
            {
                _context.OrderItems.Remove(item);
                await _context.SaveChangesAsync();
            }
        }

        #endregion
    }
}
