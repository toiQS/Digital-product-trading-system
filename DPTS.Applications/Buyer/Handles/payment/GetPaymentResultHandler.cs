using DPTS.Applications.Buyer.Queries.order;
using DPTS.Applications.Buyer.Queries.payment;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles.payment
{
    public class GetPaymentResultHandler : IRequestHandler<GetPaymentResultQuery, ServiceResult<string>>
    {
        private readonly IWalletRepository _walletRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IEscrowRepository _ecrowRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly ILogRepository _logRepository;
        private readonly IProductRepository _productRepository;
        private readonly IMediator _mediator;
        private readonly ILogger<GetPaymentResultHandler> _logger;
        
        public async Task<ServiceResult<string>> Handle(GetPaymentResultQuery request, CancellationToken cancellationToken)
        {
            var user = await _userProfileRepository.GetByUserIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogError("");
                return ServiceResult<string>.Error("");
            }
            

            var query = new GetCheckoutQuery()
            {
                BuyerId = request.UserId,

            };
            var checkOrder = await _mediator.Send(query);
            if (checkOrder.Status == StatusResult.Errored)
            {
                _logger.LogError("");
                return ServiceResult<string>.Error("");
            }

            var orders = (await _orderRepository.GetByBuyerAsync(request.UserId)).Where(x => x.IsPaid == false);
            if (orders == null)
            {
                _logger.LogError("");
                return ServiceResult<string>.Error("");
            }
            if (orders.Count() > 1)
            {
                _logger.LogError("");
                return ServiceResult<string>.Error("");
            }
            if (orders.Count() == 0)
            {
                _logger.LogError("");
                return ServiceResult<string>.Error("");
            }
            var order = orders.FirstOrDefault();
            if (order == null)
            {
                _logger.LogError("");
                return ServiceResult<string>.Error("");
            }

            var items = checkOrder.Data.Items;
            var products = await _productRepository.SearchAsync();
            var queryJoined = from item in items
                              join product in products
                              on item.ProductId equals product.ProductId
                              select new JoinedResult
                              {
                                  ProductId = item.ProductId,
                                  Amount = item.Amount,
                                  Price = item.Price,
                                  StoreId = product.StoreId,
                              };
            var escrows = new List<Escrow>();

            foreach (var item in queryJoined)
            {
                var findItem = escrows.Where(x => x.StoreId == item.StoreId).FirstOrDefault();
                if (findItem == null)
                    escrows.Add(new Escrow()
                    {
                        StoreId = item.StoreId,
                        EscrowId = Guid.NewGuid().ToString(),
                        OrderId = order.OrderId,
                        Amount = item.Amount,
                        CreatedAt = DateTime.UtcNow,
                        Expired = DateTime.UtcNow.AddDays(30),
                        ReleasedAt = DateTime.UtcNow,
                        ReleasedBy = "System",
                        UpdatedAt = DateTime.UtcNow,
                    });
                else
                {
                    findItem.Amount += item.Amount;
                    findItem.UpdatedAt = DateTime.UtcNow;
                }
            }

            var wallet = await _walletRepository.GetByIdAsync(request.PaymentMethodId);
            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(request.PaymentMethodId);
            if (paymentMethod == null && wallet == null)
            {
                _logger.LogError("");
                return ServiceResult<string>.Error("");
            }
            if (wallet != null)
            {
                
                Log log = new() { UserId = user.UserId, Action = "", CreatedAt = DateTime.UtcNow, LogId = Guid.NewGuid().ToString(), TargetId = "", TargetType = "" };
                try
                {
                    
                    order.IsPaid = true;
                    order.UpdatedAt = DateTime.UtcNow;
                    await _orderRepository.UpdateAsync(order);
                    await _logRepository.AddAsync(log);
                    
                }
                catch (Exception ex)
                {
                    _logger.LogError("");
                    return ServiceResult<string>.Error("");
                }
            }
            if (paymentMethod != null)
            {
                Log log = new() { UserId = user.UserId, Action = "", CreatedAt = DateTime.UtcNow, LogId = Guid.NewGuid().ToString(), TargetId = "", TargetType = "" };
                try
                {
                    order.IsPaid = true;
                    order.UpdatedAt = DateTime.UtcNow;
                    await _orderRepository.UpdateAsync(order);
                    await _logRepository.AddAsync(log);

                }
                catch (Exception ex)
                {
                    _logger.LogError("");
                    return ServiceResult<string>.Error("");
                }
            }
            return ServiceResult<string>.Success("");
        }
        private class JoinedResult
        {
            public string ProductId { get; set; } = string.Empty;
            public string StoreId {  get; set; } = string.Empty;
            public decimal Price { get; set; }
            public decimal Amount { get; set; }

        }
    }
}
