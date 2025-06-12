using DPTS.Domains;
using DPTS.Infrastructures.Data;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repository.Implements
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _context;

        public OrderRepository(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Order>> GetsAsync(
            string? buyerId = null,
            bool? isPaied = null,
            DateTime? fromDate = null,
            DateTime? toDate = null,
            bool includeBuyer = false,
            bool includeEscrow = false,
            bool includeComplaints = false,
            bool includeMessages = false,
            bool includeOrderItems = false)
        {
            var query = _context.Orders.AsQueryable();

            if (!string.IsNullOrWhiteSpace(buyerId))
                query = query.Where(o => o.BuyerId == buyerId);

            if (isPaied != null)
                query = query.Where(o => o.IsPaied == isPaied);

            if (fromDate != null)
                query = query.Where(o => o.CreatedAt >= fromDate);

            if (toDate != null)
                query = query.Where(o => o.CreatedAt <= toDate);

            if (includeBuyer)
                query = query.Include(o => o.Buyer);

            if (includeEscrow)
                query = query.Include(o => o.Escrow);

            if (includeComplaints)
                query = query.Include(o => o.Complaints);

            if (includeMessages)
                query = query.Include(o => o.Messages);

            if (includeOrderItems)
                query = query.Include(o => o.OrderItems);

            return await query
                .OrderByDescending(o => o.CreatedAt)
                .ToListAsync();
        }

        public async Task<Order?> GetByIdAsync(
            string id,
            bool includeBuyer = false,
            bool includeEscrow = false,
            bool includeComplaints = false,
            bool includeMessages = false,
            bool includeOrderItems = false)
        {
            var query = _context.Orders.Where(o => o.OrderId == id);

            if (includeBuyer)
                query = query.Include(o => o.Buyer);

            if (includeEscrow)
                query = query.Include(o => o.Escrow);

            if (includeComplaints)
                query = query.Include(o => o.Complaints);

            if (includeMessages)
                query = query.Include(o => o.Messages);

            if (includeOrderItems)
                query = query.Include(o => o.OrderItems);

            return await query.FirstOrDefaultAsync();
        }

        public async Task AddAsync(Order order)
        {
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Order order)
        {
            _context.Orders.Update(order);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(string id)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }
    }

}