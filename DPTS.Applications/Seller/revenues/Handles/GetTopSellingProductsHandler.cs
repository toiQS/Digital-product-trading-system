using DPTS.Applications.Seller.revenues.Dtos;
using DPTS.Applications.Seller.revenues.Queries;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DPTS.Applications.Seller.revenues.Handles
{
    public class GetTopSellingProductsHandler
    {
        private readonly IOrderRepository _orderRepository;
        private readonly IOrderItemRepository _itemRepository;
        private readonly ILogger<GetTopSellingProductsHandler> _logger;
        //public async Task<ServiceResult<IEnumerable<TopSellingProductDto>>> Handle(GetTopSellingProductsQuery query, CancellationToken cancellationToken = default)
        //{
        //    _logger.LogInformation("");
        //    try
        //    {
        //        var orders = (await _orderRepository.GetsAsync(includeEscrow: true)).Where(x => x.Escrow.SellerId == query.SellerId && x.Escrow.Status == Domains.EscrowStatus.Done).ToList();
        //        var orderItems = (await _itemRepository.GetsAsync(includeProduct: true)).Where(x => x.Product.SellerId == query.SellerId).ToList();
        //        var joinedOrderAndItems = from order in orders
        //                                  join item in orderItems on order.OrderId equals item.OrderId
        //                                  group item by item.ProductId
        //    }
        //    catch (Exception ex)
        //    {
        //        _logger.LogError("");
        //        return ServiceResult<IEnumerable<TopSellingProductDto>>.Error("");
        //    }
        //}
        public class JoinedResult
        {
            public string ProjectId { get; set; } = string.Empty;
            public string OrderId { get; set; } = string.Empty;
            public string OderItemId { get; set; } = string.Empty ;
            public int Quantity { get; set; }
        }
    }
}
