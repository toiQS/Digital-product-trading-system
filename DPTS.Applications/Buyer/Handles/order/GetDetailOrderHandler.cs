using DPTS.Applications.Buyer.Dtos.order;
using DPTS.Applications.Buyer.Queries.order;
using DPTS.Applications.Shareds;
using DPTS.Infrastructures.Repository.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DPTS.Applications.Buyer.Handles.order
{
    public class GetDetailOrderHandler : IRequestHandler<GetDetailOrderQuery, ServiceResult<DetailOrderDto>>
    {
        private readonly IEscrowRepository _escrowRepository;
        private readonly IOrderItemRepository _orderItemRepository;
        private readonly IEscrowProcessRepository _escrowProcessRepository;
        private readonly IUserProfileRepository _userProfileRepository;
        private readonly IProductRepository _productRepository;
        private readonly IStoreRepository _storeRepository;
        private readonly IProductImageRepository _productImageRepository;
        private readonly IPaymentMethodRepository _paymentMethodRepository;
        private readonly IOrderPaymentRepository _orderPaymentRepository;
        private readonly ILogger<GetDetailOrderHandler> _logger;

        public GetDetailOrderHandler(
            IEscrowRepository escrowRepository,
            IOrderItemRepository orderItemRepository,
            IEscrowProcessRepository escrowProcessRepository,
            IUserProfileRepository userProfileRepository,
            IProductRepository productRepository,
            IStoreRepository storeRepository,
            IProductImageRepository productImageRepository,
            IPaymentMethodRepository paymentMethodRepository,
            IOrderPaymentRepository orderPaymentRepository,
            ILogger<GetDetailOrderHandler> logger)
        {
            _escrowRepository = escrowRepository;
            _orderItemRepository = orderItemRepository;
            _escrowProcessRepository = escrowProcessRepository;
            _userProfileRepository = userProfileRepository;
            _productRepository = productRepository;
            _storeRepository = storeRepository;
            _productImageRepository = productImageRepository;
            _paymentMethodRepository = paymentMethodRepository;
            _orderPaymentRepository = orderPaymentRepository;
            _logger = logger;
        }

        public async Task<ServiceResult<DetailOrderDto>> Handle(GetDetailOrderQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Fetching detailed order for EscrowId={EscrowId}, UserId={UserId}", request.EscrowId, request.UserId);

            var result = new DetailOrderDto();

            // 1. Kiểm tra người dùng
            var profile = await _userProfileRepository.GetByUserIdAsync(request.UserId);
            if (profile == null)
            {
                _logger.LogError("User not found: UserId={UserId}", request.UserId);
                return ServiceResult<DetailOrderDto>.Error("Không tìm thấy người dùng.");
            }

            // 2. Lấy thông tin escrow
            var escrow = await _escrowRepository.GetByIdAsync(request.EscrowId);
            if (escrow == null)
            {
                _logger.LogError("Escrow not found: EscrowId={EscrowId}", request.EscrowId);
                return ServiceResult<DetailOrderDto>.Error("Không tìm thấy giao dịch.");
            }

            // 3. Lấy thông tin cửa hàng
            var store = await _storeRepository.GetByIdAsync(escrow.StoreId);
            if (store == null)
            {
                _logger.LogError("Store not found: StoreId={StoreId}", escrow.StoreId);
                return ServiceResult<DetailOrderDto>.Error("Không tìm thấy cửa hàng.");
            }

            // 4. Lấy các sản phẩm thuộc đơn hàng của escrow này
            var orderItems = await _orderItemRepository.GetByOrderIdAsync(escrow.OrderId);
            foreach (var item in orderItems)
            {
                var product = await _productRepository.GetByIdAsync(item.ProductId);
                if (product == null)
                {
                    _logger.LogError("Product not found: ProductId={ProductId}", item.ProductId);
                    return ServiceResult<DetailOrderDto>.Error("Không tìm thấy sản phẩm.");
                }

                // Chỉ lấy sản phẩm thuộc cùng store
                if (product.StoreId == escrow.StoreId)
                {
                    var productImage = await _productImageRepository.GetPrimaryAsync(item.ProductId);
                    if (productImage == null)
                    {
                        _logger.LogError("Product image not found: ProductId={ProductId}", item.ProductId);
                        return ServiceResult<DetailOrderDto>.Error("Không tìm thấy hình ảnh sản phẩm.");
                    }

                    result.DetailItemIndexDtos.Add(new DetailItemIndexDto
                    {
                        ProductId = item.ProductId,
                        ProductName = product.ProductName,
                        StoreName = store.StoreName,
                        ProductImage = productImage.ImagePath
                    });
                }
            }

            // 5. Lấy các bước xử lý giao dịch (EscrowProcess)
            var processes = await _escrowProcessRepository.GetByEscrowIdAsync(escrow.EscrowId);
            result.PurchasingProcesses = processes.Select(x => new PurchasingProcess
            {
                Information = x.EscrowProcessInformation,
                NameProcess = x.ProcessName,
                ProcessAt = x.ProcessAt.ToString("dd/MM/yyyy - HH:mm")
            }).ToList();

            // 6. Trạng thái tổng của giao dịch
            result.StatusOrder = EnumHandle.HandleEscrowStatus(escrow.Status);

            // 7. Lấy phương thức thanh toán đã dùng
            var orderPayment = await _orderPaymentRepository.GetByOrderIdAsync(escrow.OrderId);

            if (orderPayment == null)
            {
                _logger.LogError("OrderPayment not found: OrderId={OrderId}", escrow.OrderId);
                return ServiceResult<DetailOrderDto>.Error("Không tìm thấy thông tin thanh toán.");
            }

            var paymentMethod = await _paymentMethodRepository.GetByIdAsync(orderPayment.PaymentMethodId);
            if (paymentMethod == null)
            {
                _logger.LogError("PaymentMethod not found: PaymentMethodId={PaymentMethodId}", orderPayment.PaymentMethodId);
                return ServiceResult<DetailOrderDto>.Error("Không tìm thấy phương thức thanh toán.");
            }

            result.DetailInformation = new DetailInformation
            {
                PaymentMethodName = EnumHandle.HandlePaymentProvider(paymentMethod.Provider)
            };

            return ServiceResult<DetailOrderDto>.Success(result);
        }
    }
}
