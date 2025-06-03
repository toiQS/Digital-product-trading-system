using DPTS.Applications.Dtos;
using DPTS.Applications.Interfaces;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Data;

namespace DPTS.Applications.Implements
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ILogger<OrderService> _logger;
        public OrderService(ApplicationDbContext context, ILogger<OrderService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<OrderIndexDto>>> GetOrdersBySellerId(string sellerId, int pageNumber = 1, int pageSize = 10)
        {
            _logger.LogInformation("Fetching orders for seller {SellerId}, page {PageNumber}, size {PageSize}", sellerId, pageNumber, pageSize);

            // Truy vấn Escrows liên quan đến Seller + Order + OrderItems + Product
            var escrows = await _context.Escrows
                .Where(e => e.SellerId == sellerId)
                .Include(e => e.Order)
                    .ThenInclude(o => o.OrderItems.Where(oi => oi.Product.SellerId == sellerId))
                        .ThenInclude(oi => oi.Product)
                .OrderBy(e => e.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            if (!escrows.Any())
            {
                return ServiceResult<IEnumerable<OrderIndexDto>>.Success(Enumerable.Empty<OrderIndexDto>());
            }

            // Lấy toàn bộ buyerId cần dùng (tránh gọi từng user)
            var buyerIds = escrows.Select(e => e.Order.BuyerId).Distinct().ToList();
            var buyers = await _context.Users
                .Where(u => buyerIds.Contains(u.UserId))
                .ToDictionaryAsync(u => u.UserId);

            var result = new List<OrderIndexDto>();
            foreach (var escrow in escrows)
            {
                var buyerName = buyers.TryGetValue(escrow.Order.BuyerId, out var buyer)
                    ? buyer.FullName ?? "N/A"
                    : "N/A";

                foreach (var item in escrow.Order.OrderItems)
                {
                    // Đảm bảo là của seller (phòng trường hợp Include không lọc hết)
                    if (item.Product.SellerId != sellerId) continue;

                    result.Add(new OrderIndexDto
                    {
                        OrderId = item.OrderId,
                        ProductionName = item.Product.ProductName,
                        BuyerName = buyerName,
                        Amount = item.TotalAmount,
                        Status = EnumHandle.HandleEscrowStatus(escrow.Status),
                    });
                }
            }

            return ServiceResult<IEnumerable<OrderIndexDto>>.Success(result);
        }

    }
}
