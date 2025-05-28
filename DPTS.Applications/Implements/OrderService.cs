using DPTS.Applications.Dtos.orders;
using DPTS.Applications.Interfaces;
using DPTS.Applications.Shareds;
using DPTS.Applications.Shareds.Interfaces;
using DPTS.Domains;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace DPTS.Applications.Implements
{
    public class OrderService : IOrderService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<OrderService> _logger;
        public OrderService(IUnitOfWork unitOfWork, ILogger<OrderService> logger)
        {
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<ServiceResult<IEnumerable<OrderIndexModel>>> GetOrdersBySellerId(string sellerId)
        {
            try
            {
                _logger.LogInformation("");
                var ecsrows = await _unitOfWork.Repository<Escrow>().GetManyAsync(nameof(Escrow.SellerId), sellerId);
                if (!ecsrows.Any()) return ServiceResult<IEnumerable<OrderIndexModel>>.Success(null!);
                var products = await _unitOfWork.Repository<Product>().GetManyAsync(nameof(Product.SellerId), sellerId);
                if (!products.Any() && ecsrows.Any()) return ServiceResult<IEnumerable<OrderIndexModel>>.Error("");
                List<OrderIndexModel> orderIndexModels = new List<OrderIndexModel>();
                foreach (var ecs in ecsrows)
                {
                    var order = await _unitOfWork.Repository<Order>().GetOneAsync(nameof(Order.OrderId), ecs.OrderId);
                    if (order == null) return ServiceResult<IEnumerable<OrderIndexModel>>.Error("");
                    var buyer = await _unitOfWork.Repository<User>().GetOneAsync(nameof(User.UserId), order.BuyerId);
                    if (buyer == null) return ServiceResult<IEnumerable<OrderIndexModel>>.Error("");
                    var orderItems = await _unitOfWork.Repository<OrderItem>().GetManyAsync(nameof(OrderItem.OrderId), order.OrderId);

                    var orderIndexModel = new OrderIndexModel()
                    {
                        BuyerName = buyer.FullName,
                        Amount = ecs.Amount,
                        Status = nameof(ecs.Status),
                        OrderId = order.OrderId,
                    };
                    foreach (var orderItem in orderItems)
                    {
                        var product = await _unitOfWork.Repository<Product>().GetOneAsync(nameof(Product.ProductId), orderItem.ProductId);
                        if (product == null) return ServiceResult<IEnumerable<OrderIndexModel>>.Error("");
                        if (product.SellerId != sellerId) continue;
                        //orderIndexModel.ProductionName.AppendLine($"{product.Title},");
                        orderIndexModel.ProductionName.AppendLine(product.Title);
                    }
                    orderIndexModels.Add(orderIndexModel);
                }
                return ServiceResult<IEnumerable<OrderIndexModel>>.Success(orderIndexModels);
            }
            catch (Exception ex)
            {
                _logger.LogError("");
                return ServiceResult<IEnumerable<OrderIndexModel>>.Error("");
            }
        }

        
    }
}
