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

        #region Query Builder
        private IQueryable<Order> BuildBaseQuery(
            bool includeBuyer = false,
            bool includeItems = false,
            bool includeEscrows = false)
        {
            var query = _context.Orders.AsQueryable();

            if (includeBuyer)
                query = query.Include(o => o.Buyer);

            if (includeItems)
                query = query.Include(o => o.OrderItems);

            if (includeEscrows)
                query = query.Include(o => o.Escrows);

            return query;
        }
        #endregion

        #region Get Methods

        public async Task<Order?> GetByIdAsync(string orderId, bool includeItems = false, bool includeEscrows = false, bool includeBuyer = false)
        {
            if (string.IsNullOrWhiteSpace(orderId))
                return null;

            return await BuildBaseQuery(includeBuyer, includeItems, includeEscrows)
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
        }

        public async Task<IEnumerable<Order>> GetByBuyerAsync(string buyerId, bool onlyPaid = false, int skip = 0, int take = 50)
        {
            if (string.IsNullOrWhiteSpace(buyerId))
                return Enumerable.Empty<Order>();

            var query = _context.Orders
                .Where(o => o.BuyerId == buyerId);

            if (onlyPaid)
                query = query.Where(o => o.IsPaid);

            return await query
                .OrderByDescending(o => o.CreatedAt)
                .Skip(skip)
                .Take(take)
                .ToListAsync();
        }

        public async Task<bool> ExistsAsync(string orderId)
        {
            return await _context.Orders.AnyAsync(o => o.OrderId == orderId);
        }

        public async Task<bool> IsPaidAsync(string orderId)
        {
            var order = await _context.Orders
                .Where(o => o.OrderId == orderId)
                .Select(o => o.IsPaid)
                .FirstOrDefaultAsync();

            return order;
        }

        public async Task<decimal> GetTotalSpentAsync(string buyerId)
        {
            return await _context.Orders
                .Where(o => o.BuyerId == buyerId && o.IsPaid)
                .SumAsync(o => o.TotalAmount);
        }

        #endregion

        #region CRUD

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

        public async Task DeleteAsync(string orderId)
        {
            var order = await _context.Orders.FindAsync(orderId);
            if (order != null)
            {
                _context.Orders.Remove(order);
                await _context.SaveChangesAsync();
            }
        }

        #endregion
    }
}
