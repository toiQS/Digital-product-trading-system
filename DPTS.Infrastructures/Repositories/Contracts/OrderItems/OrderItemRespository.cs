using DPTS.Domains;
using DPTS.Infrastructures.Datas;
using Microsoft.EntityFrameworkCore;

namespace DPTS.Infrastructures.Repositories.Contracts.OrderItems
{
    public class OrderItemRespository : IOrderItemCommand, IOrderItemQuery
    {
        private readonly ApplicationDbContext _context;
        public OrderItemRespository(ApplicationDbContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }
        public async Task<IEnumerable<OrderItem>> GetOrderItemsByOrderIdsAsync(List<string> orderIds, CancellationToken cancellationToken)
        {
            var result = new List<OrderItem>();
            foreach (var orderId in orderIds)
            {
               var orderItems = await _context.OrdersItem
                    .Where(oi => oi.OrderId == orderId)
                    .ToListAsync(cancellationToken);
                result.AddRange(orderItems);
            }
            return result;
        }
    }
}
