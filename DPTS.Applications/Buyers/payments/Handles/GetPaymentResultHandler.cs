using DPTS.Applications.Buyers.payments.Dtos;
using DPTS.Applications.Buyers.payments.Queries;
using DPTS.Applications.Shareds;
using DPTS.Domains;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyers.payments.Handles
{
    public class GetPaymentResultHandler : IRequestHandler<GetPaymentResultQuery, ServiceResult<PaymentResultDto>>
    {
        private readonly IEscrowRepository _escrowRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly IProductRepository _productRepository;
        private readonly ILogger<GetPaymentResultHandler> _logger;
        private readonly IStoreRepository _storeRepository;
        private readonly ILogRepository _logRepository;

        public GetPaymentResultHandler(
            IEscrowRepository escrowRepository,
            IOrderRepository orderRepository,
            IProductRepository productRepository,
            ILogger<GetPaymentResultHandler> logger,
            IStoreRepository storeRepository,
            ILogRepository logRepository)
        {
            _escrowRepository = escrowRepository;
            _orderRepository = orderRepository;
            _productRepository = productRepository;
            _logger = logger;
            _storeRepository = storeRepository;
            _logRepository = logRepository;
        }

        public async Task<ServiceResult<PaymentResultDto>> Handle(GetPaymentResultQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Xử lý thanh toán cho BuyerId: {BuyerId}", request.BuyerId);
            try
            {
                var orders = (await _orderRepository.GetsAsync(includeOrderItems: true))
                    .Where(x => x.BuyerId == request.BuyerId && !x.IsPaid)
                    .ToList();

                if (!orders.Any() || orders.Count > 1)
                {
                    _logger.LogWarning("Không tìm thấy hoặc có nhiều hơn 1 đơn hàng chưa thanh toán cho BuyerId: {BuyerId}", request.BuyerId);
                    return ServiceResult<PaymentResultDto>.Error("Không thể xử lý đơn hàng.");
                }

                var order = orders.Single();
                var products = await _productRepository.GetsAsync();
                var items = order.OrderItems;

                var query = (
                    from item in items
                    join product in products on item.ProductId equals product.ProductId
                    select new
                    {
                        product.ProductId,
                        item.Quantity,
                        product.Price,
                        product.StoreId,
                        Amount = product.Price * item.Quantity
                    }).ToList();

                var storeGroups = query.GroupBy(x => x.StoreId);

                foreach (var storeGroup in storeGroups)
                {
                    var storeId = storeGroup.Key;
                    var totalAmount = storeGroup.Sum(x => x.Amount);

                    var escrow = new Escrow
                    {
                        EscrowId = Guid.NewGuid().ToString(),
                        StoreId = storeId,
                        OrderId = order.OrderId,
                        Amount = totalAmount,
                        CreatedAt = DateTime.UtcNow,
                        UpdatedAt = DateTime.UtcNow,
                        Status = EscrowStatus.Done,
                        Expired = DateTime.UtcNow.AddDays(30)
                    };

                    var log = new Log
                    {
                        LogId = Guid.NewGuid().ToString(),
                        UserId = request.BuyerId,
                        Action = $"Tạo giao dịch ký quỹ {escrow.EscrowId} cho store {storeId} với tổng tiền {totalAmount}",
                        CreatedAt = DateTime.UtcNow
                    };

                    try
                    {
                        await _escrowRepository.AddAsync(escrow);
                        await _logRepository.AddAsync(log);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "Lỗi khi tạo escrow cho StoreId: {StoreId}", storeId);
                        return ServiceResult<PaymentResultDto>.Error("Tạo giao dịch ký quỹ thất bại.");
                    }
                }

                try
                {
                    order.IsPaid = true;
                    order.UpdatedAt = DateTime.UtcNow;
                    await _orderRepository.UpdateAsync(order);

                    var log = new Log
                    {
                        LogId = Guid.NewGuid().ToString(),
                        UserId = request.BuyerId,
                        Action = $"Thanh toán thành công cho OrderId: {order.OrderId}",
                        CreatedAt = DateTime.UtcNow
                    };
                    await _logRepository.AddAsync(log);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Lỗi khi cập nhật trạng thái đơn hàng OrderId: {OrderId}", order.OrderId);
                    return ServiceResult<PaymentResultDto>.Error("Cập nhật đơn hàng thất bại.");
                }

                var result = new PaymentResultDto
                {
                    Amount = query.Sum(x => x.Amount),
                    PaidAt = DateTime.UtcNow,
                    PaymentMethod = "Chuyển khoản", // TODO: lấy theo phương thức thật nếu có
                    Status = "Đã thanh toán"
                };

                _logger.LogInformation("Hoàn tất xử lý thanh toán cho BuyerId: {BuyerId}", request.BuyerId);
                return ServiceResult<PaymentResultDto>.Success(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Lỗi xử lý thanh toán cho BuyerId: {BuyerId}", request.BuyerId);
                return ServiceResult<PaymentResultDto>.Error("Có lỗi xảy ra khi xử lý thanh toán.");
            }
        }
    }
}
